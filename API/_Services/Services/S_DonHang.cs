using AgileObjects.AgileMapper;
using AgileObjects.AgileMapper.Extensions;
using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helper.Mappers;
using API.Hubs;
using API.Models;
using LinqKit;
using Microsoft.AspNetCore.SignalR;
using API.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API._Services.Services
{
    public class S_DonHang : BaseServices, I_DonHang
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public S_DonHang(DBContext dbContext, IHubContext<NotificationHub> hubContext) : base(dbContext)
        {
            _hubContext = hubContext;
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
            if (dateType == 1)
                predicateDonHang = predicateDonHang.And(x => x.Create_Time.HasValue && start.Date <= x.Create_Time.Value.Date && x.Create_Time.Value.Date <= end.Date);
            else if (dateType == 2)
                predicateDonHang = predicateDonHang.And(x => x.Date.HasValue && start.Date <= x.Date.Value.Date && x.Date.Value.Date <= end.Date);
            if (!string.IsNullOrEmpty(ma_DH))
                predicateDonHang = predicateDonHang.And(x => x.Ma_DH.Contains(ma_DH));
            if (payType == 1)
                predicateDonHang = predicateDonHang.And(x => x.TienMat > 0);
            else if (payType == 2)
                predicateDonHang = predicateDonHang.And(x => x.ChuyenKhoan > 0);
            if (filter.TinhTrang == "1")
                predicateDonHang = predicateDonHang.And(x => (x.ChuyenKhoan ?? 0) + (x.TienMat ?? 0) >= (x.TongTien ?? 0));
            else if (filter.TinhTrang == "2")
                predicateDonHang = predicateDonHang.And(x => (x.ChuyenKhoan ?? 0) + (x.TienMat ?? 0) < (x.TongTien ?? 0));
            else if (filter.TinhTrang == "4")
                predicateDonHang = predicateDonHang.And(x => x.Date.HasValue && x.SoNgayCongNo != null && x.Date.Value.AddDays(x.SoNgayCongNo.Value).Date < DateTime.Now.Date && (x.ChuyenKhoan ?? 0) + (x.TienMat ?? 0) < (x.TongTien ?? 0));
            if (filter.IdNCC != null && filter.IdNCC.Count > 0)
                predicateDonHang = predicateDonHang.And(x => x.ID_NCC.HasValue && filter.IdNCC.Contains(x.ID_NCC.Value));
            if (filter.IdKH != null && filter.IdKH.Count > 0)
                predicateDonHang = predicateDonHang.And(x => x.ID_KH.HasValue && filter.IdKH.Contains(x.ID_KH.Value));
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
            var infoDict = info.ToDictionary(x => x.ID);
            List<DonHangO> result = donHangs.Select(dh =>
            {
                InfoDTO matchedInfo = null;
                int? infoId = dh.Loai == 1 ? dh.ID_NCC : dh.ID_KH;
                if (infoId.HasValue && infoDict.TryGetValue(infoId.Value, out var i))
                    matchedInfo = i;

                return new DonHangO
                {
                    ID = dh.ID,
                    ID_KH = dh.ID_KH,
                    ID_NCC = dh.ID_NCC,
                    Ten_NCC = dh.Loai == 1 ? (matchedInfo?.Ten ?? $"NCC #{dh.ID_NCC}") : null,
                    Ten_KH = dh.Loai == 2 ? (matchedInfo?.Ten ?? $"KH #{dh.ID_KH}") : null,
                    DiaChi = matchedInfo?.DiaChi,
                    Loai = dh.Loai,
                    TongTien = dh.TongTien,
                    TienMat = dh.TienMat,
                    ChuyenKhoan = dh.ChuyenKhoan,
                    ID_NV = dh.ID_NV,
                    Status = (dh.ChuyenKhoan ?? 0) + (dh.TienMat ?? 0) >= (dh.TongTien ?? 0),
                    Ma_DH = dh.Ma_DH,
                    Date = dh.Date,
                    Create_Time = dh.Create_Time,
                    SoNgayCongNo = dh.SoNgayCongNo,
                };
            }).ToList();
            result = dateType == 1
                ? result.OrderByDescending(x => x.Create_Time).ToList()
                : result.OrderByDescending(x => x.Date).ToList();
            return result;
        }
        public async Task<DonHangO> GetById(int id)
        {
            var dh = await _repoAccessor.DonHang.FindAll(x => x.ID == id).FirstOrDefaultAsync();
            if (dh == null) return null;

            string ten = "";
            if (dh.Loai == 1)
            {
                var ncc = await _repoAccessor.NhaCungCap.FindById(dh.ID_NCC);
                ten = ncc?.Ten ?? "";
            }
            else if (dh.Loai == 2)
            {
                var kh = await _repoAccessor.KhachHang.FindById(dh.ID_KH);
                ten = kh?.Ten ?? "";
            }

            return new DonHangO
            {
                ID = dh.ID,
                ID_KH = dh.ID_KH,
                ID_NCC = dh.ID_NCC,
                Ten_NCC = dh.Loai == 1 ? ten : null,
                Ten_KH = dh.Loai == 2 ? ten : null,
                Loai = dh.Loai,
                TongTien = dh.TongTien,
                TienMat = dh.TienMat,
                ChuyenKhoan = dh.ChuyenKhoan,
                ID_NV = dh.ID_NV,
                Status = (dh.ChuyenKhoan ?? 0) + (dh.TienMat ?? 0) >= (dh.TongTien ?? 0),
                Ma_DH = dh.Ma_DH,
                Date = dh.Date,
                Create_Time = dh.Create_Time,
                SoNgayCongNo = dh.SoNgayCongNo,
            };
        }
        public async Task<List<ChiTietDonHangDTO>> GetDetail(int id)
        {
            var dh = await _repoAccessor.DonHang.FirstOrDefaultAsync(x => x.ID == id, true);
            if (dh == null) return new List<ChiTietDonHangDTO>();

            var data = await _repoAccessor.ChiTietDonHang
                .FindAll(x => x.ID_DH == id)
                .Join(_repoAccessor.SanPham.FindAll(),
                    ct => ct.ID_SP,
                    sp => sp.ID,
                    (ct, sp) => new ChiTietDonHangDTO
                    {
                        ID = ct.ID,
                        ID_DH = ct.ID_DH,
                        Ma_DH = dh.Ma_DH,
                        ID_SP = ct.ID_SP,
                        Ten_SP = sp.Ten,
                        Dvt = sp.Dvt,
                        SoLuong = ct.SoLuong,
                        Gia = ct.Gia,
                        ThanhTien = ct.ThanhTien,
                        Updated_Time = ct.Updated_Time,
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
                SoNgayCongNo = model.SoNgayCongNo,
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
            dh.Date = !string.IsNullOrWhiteSpace(model.Date_Str) ? Convert.ToDateTime(model.Date_Str) : null;
            _repoAccessor.DonHang.Add(dh);
            await _repoAccessor.Save();
            var idDH = dh.ID;
            var items = model.ChiTiet.Where(x => x.ID_SP > 0 && x.SoLuong > 0).ToList();
            var spIds = items.Select(x => x.ID_SP).Distinct().ToList();

            var products = spIds.Any()
                ? await _repoAccessor.SanPham.FindAll(x => spIds.Contains(x.ID)).ToDictionaryAsync(x => x.ID)
                : new Dictionary<int, SanPham>();

            foreach (var item in items)
            {
                if (!products.TryGetValue(item.ID_SP, out var sp) || sp == null) continue;
                if (model.Loai == 1)
                    sp.SoLuong = (sp.SoLuong ?? 0) + item.SoLuong;
                else sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
                item.ID_DH = idDH;
                item.Updated_Time = now;
                _repoAccessor.ChiTietDonHang.Add(item);
            }

            await _repoAccessor.Save();

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
                SoNgayCongNo = dh.SoNgayCongNo,
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
                await NotifyOrderChanged("create", res);
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
            dh.SoNgayCongNo = model.SoNgayCongNo;
            _repoAccessor.DonHang.Update(dh);
            var items = model.ChiTiet.Where(x => x.ID_SP > 0 && x.SoLuong > 0).ToList();
            var itemSpIds = items.Select(x => x.ID_SP).Distinct().ToList();

            var existingCTs = await _repoAccessor.ChiTietDonHang.FindAll(x => x.ID_DH == model.ID).ToListAsync();
            var existingDict = existingCTs.ToDictionary(x => x.ID);
            var incomingIds = items.Where(x => x.ID > 0).Select(x => x.ID).ToHashSet();
            var removedCTs = existingCTs.Where(x => !incomingIds.Contains(x.ID)).ToList();

            var spIds = existingCTs.Select(x => x.ID_SP).Concat(itemSpIds).Distinct().ToList();
            var products = spIds.Any()
                ? await _repoAccessor.SanPham.FindAll(x => spIds.Contains(x.ID)).ToDictionaryAsync(x => x.ID)
                : new Dictionary<int, SanPham>();

            foreach (var removed in removedCTs)
            {
                if (products.TryGetValue(removed.ID_SP, out var sp) && sp != null)
                {
                    if (model.Loai == 1) sp.SoLuong = (sp.SoLuong ?? 0) - removed.SoLuong;
                    else sp.SoLuong = (sp.SoLuong ?? 0) + removed.SoLuong;
                }
                _repoAccessor.ChiTietDonHang.Remove(removed);
            }

            foreach (var item in items)
            {
                if (!products.TryGetValue(item.ID_SP, out var sp) || sp == null) continue;
                var chiTiet = existingDict.TryGetValue(item.ID, out var ct) ? ct : null;

                if (chiTiet != null)
                {
                    if (chiTiet.ID_SP != item.ID_SP)
                    {
                        if (products.TryGetValue(chiTiet.ID_SP, out var oldSp) && oldSp != null)
                        {
                            if (model.Loai == 1) oldSp.SoLuong = (oldSp.SoLuong ?? 0) - chiTiet.SoLuong;
                            else oldSp.SoLuong = (oldSp.SoLuong ?? 0) + chiTiet.SoLuong;
                        }
                        if (model.Loai == 1) sp.SoLuong = (sp.SoLuong ?? 0) + item.SoLuong;
                        else sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
                    }
                    else
                    {
                        int delta = item.SoLuong - chiTiet.SoLuong;
                        if (model.Loai == 1) sp.SoLuong = (sp.SoLuong ?? 0) + delta;
                        else sp.SoLuong = (sp.SoLuong ?? 0) - delta;
                    }
                    chiTiet.ID_SP = item.ID_SP;
                    chiTiet.SoLuong = item.SoLuong;
                    chiTiet.ThanhTien = item.ThanhTien;
                    chiTiet.Gia = item.Gia;
                    chiTiet.Updated_Time = DateTime.Now;
                }
                else
                {
                    if (model.Loai == 1) sp.SoLuong = (sp.SoLuong ?? 0) + item.SoLuong;
                    else sp.SoLuong = (sp.SoLuong ?? 0) - item.SoLuong;
                    item.ID_DH = model.ID;
                    item.Updated_Time = DateTime.Now;
                    _repoAccessor.ChiTietDonHang.Add(item);
                }
            }

            await _repoAccessor.Save();

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
                SoNgayCongNo = dh.SoNgayCongNo,
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
                await NotifyOrderChanged("update", res);
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
            var dh = await _repoAccessor.DonHang.FirstOrDefaultAsync(x => x.ID == id);
            if (dh == null) return true;
            var listChitiet = await _repoAccessor.ChiTietDonHang.FindAll(x => x.ID_DH == id).ToListAsync();
            bool saved;
            if (!listChitiet.Any())
            {
                _repoAccessor.DonHang.Remove(dh);
                saved = await _repoAccessor.Save();
                if (saved) await NotifyOrderChanged("delete", BuildRes(dh));
                return saved;
            }
            var productIds = listChitiet.Select(x => x.ID_SP).Distinct().ToList();

            var products = await _repoAccessor.SanPham
                .FindAll(x => productIds.Contains(x.ID))
                .ToDictionaryAsync(x => x.ID);

            foreach (var group in listChitiet.GroupBy(x => x.ID_SP))
            {
                if (!products.TryGetValue(group.Key, out var sp) || sp == null) continue;
                var quantity = group.Sum(x => x.SoLuong);
                sp.SoLuong = (sp.SoLuong ?? 0) + (dh.Loai == 1 ? -quantity : quantity);
            }

            _repoAccessor.ChiTietDonHang.RemoveMultiple(listChitiet);
            _repoAccessor.DonHang.Remove(dh);

            try
            {
                saved = await _repoAccessor.Save();
                if (saved)
                {
                    await NotifyOrderChanged("delete", BuildRes(dh));
                }
                return saved;
            }
            catch
            {
                return false;
            }
        }

        private DonHangO BuildRes(DonHang dh)
        {
            return new DonHangO
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
                SoNgayCongNo = dh.SoNgayCongNo,
            };
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
                if (dh.Loai == 1) sp.SoLuong = (sp.SoLuong ?? 0) - chitietDH.SoLuong;
                else sp.SoLuong = (sp.SoLuong ?? 0) + chitietDH.SoLuong;
            }
            _repoAccessor.ChiTietDonHang.Remove(chitietDH);
            try
            {
                var saved = await _repoAccessor.Save();
                return saved;
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
                var saved = await _repoAccessor.Save();
                if (saved)
                {
                    var res = await GetById(model.ID);
                    await NotifyOrderChanged("update", res);
                }
                return saved;
            }
            else return false;
        }

        public async Task<List<KeyValuePair<int, string>>> GetListNhaCungCap()
        {
            return await _repoAccessor.NhaCungCap.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.Ma_NCC})"))
                .Distinct().ToListAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListKhachHang()
        {
            return await _repoAccessor.KhachHang.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.Ma_KH})"))
                .Distinct().ToListAsync();
        }

        private async Task NotifyOrderChanged(string action, DonHangO res)
        {
            await _hubContext.Clients.All.SendAsync("OrderChanged", new
            {
                Action = action,
                Id = res.ID,
                Ma_DH = res.Ma_DH,
                Loai = res.Loai,
                TongTien = res.TongTien,
                Ten_KH = res.Ten_KH,
                Ten_NCC = res.Ten_NCC,
                Date = res.Date,
                SoNgayCongNo = res.SoNgayCongNo
            });
        }

    }
    #endregion
}