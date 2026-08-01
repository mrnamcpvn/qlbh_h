using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API._Services.Services
{
    public class S_CongNo : I_CongNo
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_CongNo(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<List<CongNoSummaryDTO>> GetSummary()
        {
            var donHangs = await _repoAccessor.DonHang
                .FindAll(x => x.Loai == 2)
                .ToListAsync();

            var ordersWithDebt = donHangs
                .Where(d => d.ID_KH.HasValue && (d.TongTien ?? 0) > (d.TienMat ?? 0) + (d.ChuyenKhoan ?? 0))
                .GroupBy(d => d.ID_KH!.Value)
                .ToList();

            if (!ordersWithDebt.Any())
                return new List<CongNoSummaryDTO>();

            var khIds = ordersWithDebt.Select(g => g.Key).ToList();
            var khachHangs = await _repoAccessor.KhachHang
                .FindAll(x => khIds.Contains(x.ID))
                .ToListAsync();
            var khDict = khachHangs.ToDictionary(x => x.ID);

            var today = DateTime.Today;
            var result = ordersWithDebt.Select(g =>
            {
                var khId = g.Key;
                var kh = khDict.GetValueOrDefault(khId);
                var orders = g.ToList();
                var soNgayCongNo = kh?.SoNgayCongNo ?? 0;

                DateTime? ngayDenHanGanNhat = null;
                var dueDates = orders
                    .Select(d => d.Date.HasValue && d.SoNgayCongNo.HasValue ? d.Date.Value.AddDays(d.SoNgayCongNo.Value) : (DateTime?)null)
                    .Where(d => d.HasValue)
                    .Select(d => d!.Value)
                    .ToList();
                if (dueDates.Any())
                    ngayDenHanGanNhat = dueDates.Min();

                return new CongNoSummaryDTO
                {
                    KhachHangID = khId,
                    TenKhachHang = kh?.Ten ?? $"KH #{khId}",
                    SDT = kh?.SDT ?? "",
                    SoNgayCongNo = soNgayCongNo,
                    SoDonNo = orders.Count,
                    TongConNo = orders.Sum(d => (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0)),
                    NgayDenHanGanNhat = ngayDenHanGanNhat,
                    TrangThai = GetTrangThai(ngayDenHanGanNhat)
                };
            })
            .OrderBy(x => x.NgayDenHanGanNhat)
            .ToList();

            return result;
        }

        public async Task<List<CongNoChiTietDTO>> GetDetail(int khachHangID)
        {
            var kh = await _repoAccessor.KhachHang.FindSingle(x => x.ID == khachHangID);
            if (kh == null)
                return new List<CongNoChiTietDTO>();

            var orders = await _repoAccessor.DonHang
                .FindAll(x => x.Loai == 2 && x.ID_KH == khachHangID)
                .Where(d => (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0) > 0)
                .OrderBy(d => d.Date)
                .ToListAsync();

            var soNgayCongNo = kh.SoNgayCongNo ?? 0;
            var today = DateTime.Today;
            return orders.Select(d =>
            {
                var ngayXuat = d.Date ?? today;
                var ngayDenHan = (DateTime?)ngayXuat.AddDays(soNgayCongNo);
                var conNo = (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0);

                return new CongNoChiTietDTO
                {
                    DonHangID = d.ID,
                    Ma_DH = d.Ma_DH,
                    NgayXuat = ngayXuat,
                    NgayDenHan = ngayDenHan,
                    TongTien = d.TongTien ?? 0,
                    DaThanhToan = (d.TienMat ?? 0) + (d.ChuyenKhoan ?? 0),
                    ConNo = conNo,
                    SoNgayQuaHan = ngayDenHan.HasValue && ngayDenHan.Value < today ? (today - ngayDenHan.Value).Days : 0
                };
            }).ToList();
        }

        public async Task<List<CongNoCustomerDTO>> GetData(CongNoFilterDTO filter)
        {
            DateTime? fromDate = null, toDate = null;
            if (DateTime.TryParse(filter?.FromDate, out var fd)) fromDate = fd;
            if (DateTime.TryParse(filter?.ToDate, out var td)) toDate = td;

            var donHangQuery = _repoAccessor.DonHang.FindAll(x => x.Loai == 2);

            if (fromDate.HasValue)
                donHangQuery = donHangQuery.Where(d => d.Date >= fromDate.Value);
            if (toDate.HasValue)
                donHangQuery = donHangQuery.Where(d => d.Date <= toDate.Value.AddDays(1));

            var donHangs = await donHangQuery.ToListAsync();

            var ordersWithDebtByKH = donHangs
                .Where(d => d.ID_KH.HasValue && (d.TongTien ?? 0) > (d.TienMat ?? 0) + (d.ChuyenKhoan ?? 0))
                .GroupBy(d => d.ID_KH!.Value)
                .ToList();

            if (!ordersWithDebtByKH.Any())
                return new List<CongNoCustomerDTO>();

            var khIds = ordersWithDebtByKH.Select(g => g.Key).ToList();
            if (filter?.IdKH?.Any() == true)
                khIds = khIds.Intersect(filter.IdKH).ToList();

            var khachHangs = await _repoAccessor.KhachHang
                .FindAll(x => khIds.Contains(x.ID))
                .ToListAsync();
            var khDict = khachHangs.ToDictionary(x => x.ID);

            var today = DateTime.Today;
            var result = new List<CongNoCustomerDTO>();

            foreach (var g in ordersWithDebtByKH)
            {
                var khId = g.Key;
                if (filter?.IdKH?.Any() == true && !filter.IdKH.Contains(khId)) continue;

                var kh = khDict.GetValueOrDefault(khId);
                var orders = g.ToList();
                var soNgayCongNo = kh?.SoNgayCongNo ?? 0;

                DateTime? ngayDenHanGanNhat = null;
                var dueDates = orders
                    .Select(d => d.Date.HasValue && d.SoNgayCongNo.HasValue ? d.Date.Value.AddDays(d.SoNgayCongNo.Value) : (DateTime?)null)
                    .Where(d => d.HasValue)
                    .Select(d => d!.Value)
                    .ToList();
                if (dueDates.Any())
                    ngayDenHanGanNhat = dueDates.Min();

                var trangThai = GetTrangThai(ngayDenHanGanNhat);

                bool isOverdue(Models.DonHang o)
                {
                    if (o.Date.HasValue && o.SoNgayCongNo.HasValue) return o.Date.Value.AddDays(o.SoNgayCongNo.Value).Date < today;
                    if (o.Date.HasValue) return o.Date.Value.AddDays(soNgayCongNo).Date < today;
                    return false;
                }

                if (filter?.TrangThai == "QuaHan" && !orders.Any(o => isOverdue(o))) continue;
                if (filter?.TrangThai == "ConHan" && !orders.Any(o => !isOverdue(o))) continue;

                var detailList = orders.Select(d =>
                {
                    var ngayXuat = d.Date ?? today;
                    DateTime? ngayDenHan = d.Date.HasValue && d.SoNgayCongNo.HasValue ? d.Date.Value.AddDays(d.SoNgayCongNo.Value) : null;
                    var conNo = (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0);
                    return new CongNoChiTietDTO
                    {
                        DonHangID = d.ID,
                        Ma_DH = d.Ma_DH,
                        NgayXuat = ngayXuat,
                        NgayDenHan = ngayDenHan,
                        TongTien = d.TongTien ?? 0,
                        DaThanhToan = (d.TienMat ?? 0) + (d.ChuyenKhoan ?? 0),
                        ConNo = conNo,
                        SoNgayQuaHan = ngayDenHan.HasValue && ngayDenHan.Value < today ? (today - ngayDenHan.Value).Days : 0
                    };
                }).OrderBy(x => x.NgayDenHan).ToList();

                result.Add(new CongNoCustomerDTO
                {
                    KhachHangID = khId,
                    TenKhachHang = kh?.Ten ?? $"KH #{khId}",
                    SDT = kh?.SDT ?? "",
                    SoNgayCongNo = soNgayCongNo,
                    HanMucCongNo = kh?.HanMucCongNo,
                    SoDonNo = orders.Count,
                    TongConNo = orders.Sum(d => (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0)),
                    NgayDenHanGanNhat = ngayDenHanGanNhat,
                    TrangThai = trangThai,
                    Orders = detailList
                });
            }

            return result.OrderBy(x => x.NgayDenHanGanNhat).ToList();
        }

        public async Task<CustomerDebtInfoDTO> GetCustomerDebtInfo(int khachHangID)
        {
            var kh = await _repoAccessor.KhachHang.FindSingle(x => x.ID == khachHangID);
            if (kh == null)
                return new CustomerDebtInfoDTO();

            var today = DateTime.Today;
            var soNgay = kh.SoNgayCongNo ?? 0;

            var orders = await _repoAccessor.DonHang
                .FindAll(x => x.Loai == 2 && x.ID_KH == khachHangID)
                .Where(d => (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0) > 0)
                .ToListAsync();

            var currentDebt = orders.Sum(d => (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0));
            var overdueOrders = orders.Where(d =>
            {
                if (!d.Date.HasValue || soNgay <= 0) return false;
                return d.Date.Value.AddDays(soNgay) < today;
            }).ToList();

            var creditLimit = kh.HanMucCongNo;
            var isOverLimit = creditLimit.HasValue && creditLimit > 0 && currentDebt > creditLimit.Value;
            var remainingCredit = creditLimit.HasValue && creditLimit > 0
                ? Math.Max(0, creditLimit.Value - currentDebt)
                : 0;

            return new CustomerDebtInfoDTO
            {
                CurrentDebt = currentDebt,
                CreditLimit = creditLimit,
                SoNgayCongNo = soNgay,
                HasOverdueOrders = overdueOrders.Any(),
                OverdueOrderCount = overdueOrders.Count,
                IsOverLimit = isOverLimit,
                RemainingCredit = remainingCredit
            };
        }

        private static string GetTrangThai(DateTime? ngayDenHan)
        {
            if (ngayDenHan == null) return "ChuaXacDinh";
            var today = DateTime.Today;
            var diff = (ngayDenHan.Value - today).Days;
            if (diff < 0) return "QuaHan";
            if (diff <= 7) return "SapDenHan";
            return "ConHan";
        }
    }
}
