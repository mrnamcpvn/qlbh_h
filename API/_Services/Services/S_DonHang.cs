using AgileObjects.AgileMapper;
using AgileObjects.AgileMapper.Extensions;
using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helper.Mappers;
using API.Helper.Utilities;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_DonHang : I_DonHang
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_DonHang(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }
        #region Download
        public async Task<OperationResult> DownloadExcel(DonHangRequestDTO filter)
        {
            var data = await GetData(filter);
            if (!data.Any())
                return new OperationResult(false, "No Data");
            foreach (var item in data)
            {
                item.StatusName = (item.TienMat ?? 0) + (item.ChuyenKhoan ?? 0) >= (item.TongTien ?? 0) ? "Đã thanh toán" : "Chưa thanh toán";
            }
            var excelResult = ExcelUtility.DownloadExcel(data, $"Resources\\Template\\DonHang\\Download_{(filter.Loai == 1 ? "Nhap" : "Xuat")}.xlsx");
            return new OperationResult(excelResult.IsSuccess, excelResult.Error, excelResult.Result);
        }
        #endregion
        #region Getdata
        public async Task<DonHangPaginationResult> GetDataPagination(DonHangRequestDTO filter)
        {
            var donHangs = await GetData(filter);
            var pageNumber = filter.Pagination?.PageNumber ?? 1;
            var pageSize = filter.Pagination?.PageSize ?? 10;
            var result = PaginationUtility<DonHangO>.Create(donHangs, pageNumber, pageSize);
            var total = donHangs.Sum(x => x.TongTien ?? 0);
            return new DonHangPaginationResult { Pagination = result, TotalAmount = total };
        }

        private async Task<List<DonHangO>> GetData(DonHangRequestDTO filter)
        {
            string fromDate = filter.FromDate;
            string toDate = filter.ToDate;
            int type = filter.Loai;
            string ma_DH = filter.Ma_DH;
            int? payType = filter.PayType;
            int? dateType = filter.DateType;
            var predicateDonHang = PredicateBuilder.New<DonHang>(x => x.Loai == type);

            var start = Convert.ToDateTime(fromDate);
            var end = Convert.ToDateTime(toDate);
            if (dateType == 1) // Ngày Lập
                predicateDonHang.And(x => x.Create_Time.HasValue && start.Date <= x.Create_Time.Value.Date && x.Create_Time.Value.Date <= end.Date);
            else if (dateType == 2) // Ngày Xuất/Nhập
                predicateDonHang.And(x => x.Date.HasValue && start.Date <= x.Date.Value.Date && x.Date.Value.Date <= end.Date);
            if (!string.IsNullOrEmpty(ma_DH))
                predicateDonHang.And(x => x.Ma_DH.Contains(ma_DH));
            if (payType == 1)
                predicateDonHang.And(x => x.TienMat > 0);
            else if (payType == 2)
                predicateDonHang.And(x => x.ChuyenKhoan > 0);
            if (filter.TinhTrang == "1")
                predicateDonHang.And(x => (x.ChuyenKhoan ?? 0) + (x.TienMat ?? 0) >= (x.TongTien ?? 0));
            else if (filter.TinhTrang == "2")
                predicateDonHang.And(x => (x.ChuyenKhoan ?? 0) + (x.TienMat ?? 0) < (x.TongTien ?? 0));
            var donHangs = await _repoAccessor.DonHang.FindAll(predicateDonHang).ToListAsync();
            List<InfoDTO> info = new();
            if (type == 1)
            {
                var ncc = donHangs.Select(x => x.ID_NCC);
                info = await _repoAccessor.NhaCungCap.FindAll(x => ncc.Contains(x.ID))
                .Select(x => new InfoDTO
                {
                    ID = x.ID,
                    Ten = x.Ten,
                    DiaChi = x.DiaChi
                }).ToListAsync();
            }
            else if (type == 2)
            {
                var kh = donHangs.Select(x => x.ID_KH);
                info = await _repoAccessor.KhachHang.FindAll(x => kh.Contains(x.ID))
                .Select(x => new InfoDTO
                {
                    ID = x.ID,
                    Ten = x.Ten,
                    DiaChi = x.DiaChi
                }).ToListAsync();

            }
            List<DonHangO> result = donHangs
                .Join(info,
                    x => x.Loai == 1 ? x.ID_NCC : x.ID_KH,
                    y => y.ID,
                    (x, y) => new { donHang = x, info = y })
                .Select(x => new DonHangO
                {
                    ID = x.donHang.ID,
                    ID_KH = x.donHang.ID_KH,
                    ID_NCC = x.donHang.ID_NCC,
                    Ten_NCC = x.donHang.Loai == 1 ? x.info.Ten : null,
                    Ten_KH = x.donHang.Loai == 2 ? x.info.Ten : null,
                    DiaChi = x.info.DiaChi,
                    Loai = x.donHang.Loai,
                    TongTien = x.donHang.TongTien,
                    TienMat = x.donHang.TienMat,
                    ChuyenKhoan = x.donHang.ChuyenKhoan,
                    ID_NV = x.donHang.ID_NV,
                    Status = (x.donHang.ChuyenKhoan ?? 0) + (x.donHang.TienMat ?? 0) >= (x.donHang.TongTien ?? 0),
                    Ma_DH = x.donHang.Ma_DH,
                    Date = x.donHang.Date, // Ngày Nhập/Xuất Hàng
                    Create_Time = x.donHang.Create_Time
                }).ToList();
            result = dateType == 1
                ? result.OrderByDescending(x => x.Create_Time).ToList()
                : result.OrderByDescending(x => x.Date).ToList();
            return result;
        }
        public async Task<List<ChiTietDonHangDTO>> GetDetail(int id)
        {
            var data = await _repoAccessor.ChiTietDonHang
                .FindAll(x => x.ID_DH == id)
                .Join(_repoAccessor.SanPham.FindAll(),
                    ct => ct.ID_SP,
                    sp => sp.ID,
                    (ct, sp) => new { ct, sp })
                .Join(_repoAccessor.DonHang.FindAll(),
                    temp => temp.ct.ID_DH,
                    dh => dh.ID,
                    (temp, dh) => new ChiTietDonHangDTO
                    {
                        ID = temp.ct.ID,
                        ID_DH = temp.ct.ID_DH,
                        Ma_DH = dh.Ma_DH,
                        ID_SP = temp.ct.ID_SP,
                        Ten_SP = temp.sp.Ten,
                        Dvt = temp.sp.Dvt,
                        SoLuong = temp.ct.SoLuong,
                        Gia = temp.ct.Gia,
                        ThanhTien = temp.ct.ThanhTien,
                        Updated_Time = temp.ct.Updated_Time,
                        SL_Ton_Dau = temp.ct.SL_Ton_Dau,
                        SL_Ton_Cuoi = temp.ct.SL_Ton_Cuoi,
                    })
                .AsNoTracking()
                .ToListAsync();

            return data;
        }
        #endregion
        #region CUD
        public async Task<DonHangO> Create(DonHangDTO model)
        {
            var now = DateTime.Now;
            DonHang dh = new()
            {
                Date = !string.IsNullOrWhiteSpace(model.Date_Str) ? Convert.ToDateTime(model.Date_Str) : null,
                TongTien = model.TongTien,
                Loai = model.Loai,
                ID_NV = model.ID_NV,
                Create_Time = now
            };
            if (model.Loai == 1)
            {
                dh.ID_NCC = model.ID_NCC;
            }
            else if (model.Loai == 2)
            {
                dh.ID_KH = model.ID_KH;
                // Ma_DH (BH + YYMMDD + XXXX)
                var p = "BH" + now.ToString("yyMMdd");
                var last = await _repoAccessor.DonHang.FindAll(x => x.Ma_DH.StartsWith(p)).Select(x => x.Ma_DH).MaxAsync();
                dh.Ma_DH = p + (last == null ? 1 : int.Parse(last[p.Length..]) + 1).ToString("D4");
            }
            _repoAccessor.DonHang.Add(dh);
            await _repoAccessor.Save();
            var idDH = dh.ID;
            var items = model.ChiTiet.Where(x => x.ID_SP > 0 && x.SoLuong > 0).ToList();
            foreach (var item in items)
            {
                var sp = await _repoAccessor.SanPham.FindById(item.ID_SP);
                if (sp == null) continue;
                item.SL_Ton_Dau = sp.SoLuong ?? 0;
                if (model.Loai == 1)
                    sp.SoLuong = (sp.SoLuong ?? 0) + item.SoLuong;
                else sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
                item.ID_DH = idDH;
                item.SL_Ton_Cuoi = sp.SoLuong;
                item.Updated_Time = now;
                _repoAccessor.ChiTietDonHang.Add(item);
                _repoAccessor.SanPham.Update(sp);
            }
            DonHangO res = new()
            {
                ID = dh.ID,
                Date = dh.Date,
                ID_KH = dh.ID_KH,
                ID_NCC = dh.ID_NCC,
                TongTien = dh.TongTien,
                Loai = dh.Loai,
                ID_NV = dh.ID_NV,
                Ma_DH = dh.Ma_DH,
                TienMat = dh.TienMat,
                ChuyenKhoan = dh.ChuyenKhoan,
                Create_Time = dh.Create_Time,
            };
            if (model.Loai == 1)
            {
                var ncc = await _repoAccessor.NhaCungCap.FindById(model.ID_NCC);
                if (ncc != null)
                {
                    res.Ten_NCC = ncc.Ten;
                    res.DiaChi = ncc.DiaChi;
                }
            }
            else if (model.Loai == 2)
            {
                var kh = await _repoAccessor.KhachHang.FindById(model.ID_KH);
                if (kh != null)
                {
                    res.Ten_KH = kh.Ten;
                    res.DiaChi = kh.DiaChi;
                }
            }
            var nv = await _repoAccessor.NhanVien.FindById(model.ID_NV);
            if (nv != null)
                res.Ten_NV = nv.Ten;
            try
            {
                await _repoAccessor.Save();
                return res;
            }
            catch
            {
                // Rollback: remove the main DonHang and all ChiTietDonHang added
                _repoAccessor.DonHang.Remove(dh);
                _repoAccessor.ChiTietDonHang.RemoveMultiple(model.ChiTiet);
                await _repoAccessor.Save();
                throw; // or return null, but better throw to let controller handle
            }
        }

        public async Task<DonHangO> Update(DonHangDTO model)
        {
            var dh = await _repoAccessor.DonHang.FindById(model.ID);
            if (dh == null)
                throw new Exception("Đơn hàng không tồn tại.");
            if (model.Loai == 1)
            {
                dh.ID_NCC = model.ID_NCC;
            }
            else if (model.Loai == 2)
            {
                dh.ID_KH = model.ID_KH;
            }
            dh.TongTien = model.TongTien;
            dh.ID_NV = model.ID_NV;
            dh.Ma_DH = model.Ma_DH;
            dh.Date = !string.IsNullOrWhiteSpace(model.Date_Str) ? Convert.ToDateTime(model.Date_Str) : null;
            _repoAccessor.DonHang.Update(dh);
            var items = model.ChiTiet.Where(x => x.ID_SP > 0 && x.SoLuong > 0).ToList();
            foreach (var item in items)
            {
                var sp = await _repoAccessor.SanPham.FindById(item.ID_SP);
                if (sp == null) continue;
                var chiTiet = await _repoAccessor.ChiTietDonHang.FindById(item.ID);

                if (chiTiet != null)
                {
                    if (chiTiet.ID_SP == item.ID_SP)
                    {
                        int delta = item.SoLuong - chiTiet.SoLuong;
                        if (model.Loai == 1)
                            sp.SoLuong = (sp.SoLuong ?? 0) + delta;
                        else sp.SoLuong = (sp.SoLuong ?? 0) - delta;
                    }
                    else
                    {
                        var oldSp = await _repoAccessor.SanPham.FindById(chiTiet.ID_SP);
                        if (oldSp != null)
                        {
                            if (model.Loai == 1) oldSp.SoLuong -= chiTiet.SoLuong;
                            else oldSp.SoLuong += chiTiet.SoLuong;
                            _repoAccessor.SanPham.Update(oldSp);
                        }
                        if (model.Loai == 1)
                            sp.SoLuong = (sp.SoLuong ?? 0) + item.SoLuong;
                        else sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
                    }
                    chiTiet.ID_SP = item.ID_SP;
                    chiTiet.SoLuong = item.SoLuong;
                    chiTiet.ThanhTien = item.ThanhTien;
                    chiTiet.Gia = item.Gia;
                    chiTiet.SL_Ton_Cuoi = sp.SoLuong;
                    chiTiet.Updated_Time = DateTime.Now;
                    _repoAccessor.ChiTietDonHang.Update(chiTiet);
                }
                else
                {
                    item.SL_Ton_Dau = sp.SoLuong ?? 0;
                    if (model.Loai == 1)
                        sp.SoLuong = (sp.SoLuong ?? 0) + item.SoLuong;
                    else sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
                    item.ID_DH = model.ID;
                    item.SL_Ton_Cuoi = sp.SoLuong;
                    item.Updated_Time = DateTime.Now;
                    _repoAccessor.ChiTietDonHang.Add(item);
                }
                _repoAccessor.SanPham.Update(sp);
            }
            DonHangO res = new()
            {
                ID = dh.ID,
                Date = dh.Date,
                ID_KH = dh.ID_KH,
                ID_NCC = dh.ID_NCC,
                TongTien = dh.TongTien,
                Loai = dh.Loai,
                ID_NV = dh.ID_NV,
                Ma_DH = dh.Ma_DH,
                TienMat = dh.TienMat,
                ChuyenKhoan = dh.ChuyenKhoan,
                Create_Time = dh.Create_Time,
            };
            if (model.Loai == 1)
            {
                var ncc = await _repoAccessor.NhaCungCap.FindById(model.ID_NCC);
                if (ncc != null)
                {
                    res.Ten_NCC = ncc.Ten;
                    res.DiaChi = ncc.DiaChi;
                }
            }
            else if (model.Loai == 2)
            {
                var kh = await _repoAccessor.KhachHang.FindById(model.ID_KH);
                if (kh != null)
                {
                    res.Ten_KH = kh.Ten;
                    res.DiaChi = kh.DiaChi;
                }
            }
            var nv = await _repoAccessor.NhanVien.FindById(model.ID_NV);
            if (nv != null)
                res.Ten_NV = nv.Ten;
            try
            {
                await _repoAccessor.Save();
                return res;
            }
            catch
            {
                return new DonHangO();
            }
        }

        public async Task<bool> Delete(int id)
        {
            // 1. Lấy đơn hàng và chi tiết
            var dh = await _repoAccessor.DonHang.FindSingle(x => x.ID == id);
            if (dh == null) return true; // Đơn hàng đã không tồn tại, coi như đã xóa thành công
            var listChitiet = await _repoAccessor.ChiTietDonHang.FindAll(x => x.ID_DH == id).ToListAsync();
            if (!listChitiet.Any())
            {
                _repoAccessor.DonHang.Remove(dh); // Xóa đơn trống
                return await _repoAccessor.Save();
            }
            var productDeltas = listChitiet.GroupBy(x => x.ID_SP)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.SoLuong));
            // 3. Lấy tất cả Sản phẩm cần cập nhật trong 1 lần duy nhất
            var productIds = productDeltas.Keys.ToList();
            var listSanPham = await _repoAccessor.SanPham.FindAll(x => productIds.Contains(x.ID)).ToListAsync();
            // 4. Lấy các bản ghi lịch sử mới nhất của các sản phẩm này (Loại trừ đơn hàng đang xóa)
            // Dùng GroupBy để lấy ID lớn nhất của từng SP trong 1 query
            var latestCTs = await _repoAccessor.ChiTietDonHang
                .FindAll(x => productIds.Contains(x.ID_SP) && x.ID_DH != id)
                .GroupBy(x => x.ID_SP)
                .Select(g => g.OrderByDescending(x => x.ID).FirstOrDefault())
                .ToListAsync();
            var validLatestCTs = latestCTs.Where(x => x != null).ToList();
            // 5. Tạo Dictionary, Xử lý logic in-memory
            var spDict = listSanPham.ToDictionary(x => x.ID);
            var lastRecordDict = validLatestCTs.ToDictionary(x => x.ID_SP);
            int multiplier = (dh.Loai == 1) ? -1 : 1; // Nhập (1) => Trừ, Xuất (2) => Cộng
            foreach (var delta in productDeltas)
            {
                if (spDict.TryGetValue(delta.Key, out var sp))
                {
                    // Cập nhật tồn kho
                    sp.SoLuong = (sp.SoLuong ?? 0) + (multiplier * delta.Value);
                    // Cập nhật SL_Ton_Cuoi cho bản ghi lịch sử mới nhất của SP đó
                    if (lastRecordDict.TryGetValue(delta.Key, out var lastRecord))
                        lastRecord.SL_Ton_Cuoi = sp.SoLuong;
                }
            }
            try
            {
                // 6. Thực thi cập nhật và xóa hàng loạt
                _repoAccessor.SanPham.UpdateMultiple(listSanPham);
                _repoAccessor.ChiTietDonHang.UpdateMultiple(validLatestCTs);
                _repoAccessor.ChiTietDonHang.RemoveMultiple(listChitiet);
                _repoAccessor.DonHang.Remove(dh);
                return await _repoAccessor.Save();
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteItem(int id)
        {
            var chitietDH = await _repoAccessor.ChiTietDonHang.FindById(id);
            if (chitietDH == null)
                return false;
            var dh = await _repoAccessor.DonHang.FindById(chitietDH.ID_DH);
            if (dh == null)
                return false;
            var sp = await _repoAccessor.SanPham.FindById(chitietDH.ID_SP);
            if (sp != null)
            {
                if (dh.Loai == 1) sp.SoLuong -= chitietDH.SoLuong;
                else sp.SoLuong += chitietDH.SoLuong;
                _repoAccessor.SanPham.Update(sp);
            }
            _repoAccessor.ChiTietDonHang.Remove(chitietDH);
            try
            {
                return await _repoAccessor.Save();
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Payment
        public async Task<bool> UpdatePayment(DonHang model)
        {
            var item = await _repoAccessor.DonHang.FindById(model.ID);
            if (item != null)
            {
                item.TienMat = model.TienMat;
                item.ChuyenKhoan = model.ChuyenKhoan;
                _repoAccessor.DonHang.Update(item);
                return await _repoAccessor.Save();
            }
            else return false;
        }
    }
    #endregion
}