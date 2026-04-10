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
            var data = await GetData(filter).ToListAsync();
            if (!data.Any())
                return new OperationResult(false, "No Data");

            foreach (var item in data)
            {
                item.StatusName = (item.TienMat ?? 0) + (item.ChuyenKhoan ?? 0) >= (item.TongTien ?? 0) ? "Đã thanh toán" : "Chưa thanh toán";
            }
            var excelResult = ExcelUtility.DownloadExcel(data, "Resources\\Template\\DonHang\\Download.xlsx");
            return new OperationResult(excelResult.IsSuccess, excelResult.Error, excelResult.Result);
        }
        #endregion
        #region Getdata
        public async Task<DonHangPaginationResult> GetDataPagination(DonHangRequestDTO filter)
        {
            var donHangs = GetData(filter);
            var pageNumber = filter.Pagination?.PageNumber ?? 1;
            var pageSize = filter.Pagination?.PageSize ?? 10;
            var donHangsList = await donHangs.ToListAsync();
            var result = PaginationUtility<DonHangO>.Create(donHangsList, pageNumber, pageSize);
            var total = donHangsList.Sum(x => x.TongTien ?? 0);
            return new DonHangPaginationResult { Pagination = result, TotalAmount = total };
        }

        private IQueryable<DonHangO> GetData(DonHangRequestDTO filter)
        {
            string fromDate = filter.FromDate;
            string toDate = filter.ToDate;
            int type = filter.Loai;
            string ma_DH = filter.Ma_DH;
            int? payType = filter.PayType;
            var predicateUser = PredicateBuilder.New<DonHang>(true);

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                var start = Convert.ToDateTime(fromDate);
                var end = Convert.ToDateTime(toDate).AddDays(1);
                predicateUser.And(x => start <= x.Date && x.Date <= end);
            }
            if (type > 0)
                predicateUser.And(x => x.Loai == type);
            if (!string.IsNullOrEmpty(ma_DH))
                predicateUser.And(x => x.Ma_DH == ma_DH);
            if (payType == 1)
                predicateUser.And(x => x.TienMat > 0);
            else if (payType == 2)
                predicateUser.And(x => x.ChuyenKhoan > 0);
            if (filter.TinhTrang == "1")
                predicateUser.And(x => (x.ChuyenKhoan ?? 0) + (x.TienMat ?? 0) >= (x.TongTien ?? 0));
            else if (filter.TinhTrang == "2")
                predicateUser.And(x => (x.ChuyenKhoan ?? 0) + (x.TienMat ?? 0) < (x.TongTien ?? 0));

            var donHangs = _repoAccessor.DonHang.FindAll(predicateUser)
                            .Join(_repoAccessor.KhachHang.FindAll(),
                                x => x.ID_KH,
                                y => y.ID,
                                (x, y) => new { donHang = x, khachHang = y }
                            ).Select(x => new DonHangO
                            {
                                ID = x.donHang.ID,
                                ID_KH = x.donHang.ID_KH,
                                Ten_KH = x.donHang.Ten_KH,
                                DiaChi = x.khachHang.DiaChi,
                                Loai = x.donHang.Loai,
                                TongTien = x.donHang.TongTien,
                                Date = x.donHang.Date,
                                TienMat = x.donHang.TienMat,
                                ChuyenKhoan = x.donHang.ChuyenKhoan,
                                ID_NV = x.donHang.ID_NV,
                                Status = (x.donHang.ChuyenKhoan ?? 0) + (x.donHang.TienMat ?? 0) >= (x.donHang.TongTien ?? 0),
                                Ma_DH = x.donHang.Ma_DH
                            }).OrderByDescending(x => x.Date);
            return donHangs;
        }
        public async Task<List<ChiTietDonHangDTO>> GetDetail(int id)
        {
            var data = await _repoAccessor.ChiTietDonHang.FindAll(x => x.ID_DH == id)
            .Join(_repoAccessor.SanPham.FindAll(),
                ct => ct.ID_SP,
                sp => sp.ID,
                (ct, sp) => new ChiTietDonHangDTO
                {
                    ID = ct.ID,
                    ID_DH = ct.ID_DH,
                    ID_SP = ct.ID_SP,
                    Ten_SP = sp.Ten,
                    Dvt = sp.Dvt,
                    SoLuong = ct.SoLuong,
                    Gia = ct.Gia,
                    ThanhTien = ct.ThanhTien,
                    Updated_Time = ct.Updated_Time,
                    SL_Ton_Dau = ct.SL_Ton_Dau,
                    SL_Ton_Cuoi = ct.SL_Ton_Cuoi,
                }).AsNoTracking().ToListAsync();
            return data;
        }
        #endregion
        #region CUD
        public async Task<DonHang> Create(DonHangDTO model)
        {
            DonHang dh = new()
            {
                Date = model.Date ?? DateTime.Now,
                ID_KH = model.ID_KH,
                Ten_KH = model.Ten_KH,
                TongTien = model.TongTien,
                Loai = model.Loai,
                ID_NV = model.ID_NV,
                Ma_DH = model.Ma_DH
            };
            _repoAccessor.DonHang.Add(dh);
            await _repoAccessor.Save();
            var idDH = dh.ID;
            foreach (var item in model.ChiTiet)
            {
                var sp = await _repoAccessor.SanPham.FindById(item.ID_SP);
                item.SL_Ton_Dau = sp.SoLuong ?? 0;
                if (model.Loai == 1)
                    sp.SoLuong = (sp.SoLuong ?? 0) + item.SoLuong;
                else sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
                item.ID_DH = idDH;
                item.SL_Ton_Cuoi = sp.SoLuong;
                item.Updated_Time = DateTime.Now;
                _repoAccessor.ChiTietDonHang.Add(item);
                _repoAccessor.SanPham.Update(sp);
            }

            try
            {
                await _repoAccessor.Save();
                return dh;
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

        public async Task<DonHang> Update(DonHangDTO model)
        {
            var dh = await _repoAccessor.DonHang.FindById(model.ID);
            if (dh == null)
                throw new Exception("Đơn hàng không tồn tại.");
            dh.ID_KH = model.ID_KH;
            dh.Ten_KH = model.Ten_KH;
            dh.TongTien = model.TongTien;
            dh.ID_NV = model.ID_NV;
            dh.Ma_DH = model.Ma_DH;
            _repoAccessor.DonHang.Update(dh);
            foreach (var item in model.ChiTiet)
            {
                var sp = await _repoAccessor.SanPham.FindById(item.ID_SP);
                var chiTiet = await _repoAccessor.ChiTietDonHang.FindById(item.ID);

                if (chiTiet != null)
                {
                    if (model.Loai == 1)
                        sp.SoLuong = (sp.SoLuong ?? 0) + (item.SoLuong - chiTiet.SoLuong);
                    else sp.SoLuong = (sp.SoLuong ?? 0) - (item.SoLuong - chiTiet.SoLuong);
                    chiTiet.SL_Ton_Cuoi = sp.SoLuong;
                    chiTiet.Updated_Time = DateTime.Now;
                    chiTiet.SoLuong = item.SoLuong;
                    chiTiet.ThanhTien = item.ThanhTien;
                    chiTiet.Gia = item.Gia;
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
            try
            {
                await _repoAccessor.Save();
                return dh;
            }
            catch
            {
                return new DonHang();
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