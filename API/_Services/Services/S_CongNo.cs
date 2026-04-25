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
            var khachHangs = await _repoAccessor.KhachHang
                .FindAll(x => x.SoNgayCongNo.HasValue && x.SoNgayCongNo > 0)
                .ToListAsync();

            var donHangs = await _repoAccessor.DonHang
                .FindAll(x => x.Loai == 2)
                .ToListAsync();

            var result = khachHangs.Select(kh =>
            {
                var orders = donHangs.Where(d => d.ID_KH == kh.ID);
                var ordersWithDebt = orders.Where(d =>
                    (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0) > 0).ToList();

                if (!ordersWithDebt.Any()) return null;

                var ngayDenHanList = ordersWithDebt
                    .Where(d => d.Date.HasValue)
                    .Select(d => d.Date.Value.AddDays(kh.SoNgayCongNo ?? 0))
                    .ToList();

                var ngayDenHanGanNhat = ngayDenHanList.Any() ? ngayDenHanList.Min() : (DateTime?)null;

                return new CongNoSummaryDTO
                {
                    KhachHangID = kh.ID,
                    TenKhachHang = kh.Ten,
                    SDT = kh.SDT,
                    SoNgayCongNo = kh.SoNgayCongNo ?? 0,
                    SoDonNo = ordersWithDebt.Count,
                    TongConNo = ordersWithDebt.Sum(d => (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0)),
                    NgayDenHanGanNhat = ngayDenHanGanNhat,
                    TrangThai = GetTrangThai(ngayDenHanGanNhat)
                };
            })
            .Where(x => x != null)
            .OrderBy(x => x.NgayDenHanGanNhat)
            .ToList();

            return result;
        }

        public async Task<List<CongNoChiTietDTO>> GetDetail(int khachHangID)
        {
            var kh = await _repoAccessor.KhachHang.FindSingle(x => x.ID == khachHangID);
            if (kh == null || !(kh.SoNgayCongNo > 0))
                return new List<CongNoChiTietDTO>();

            var orders = await _repoAccessor.DonHang
                .FindAll(x => x.Loai == 2 && x.ID_KH == khachHangID)
                .Where(d => (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0) > 0)
                .OrderBy(d => d.Date)
                .ToListAsync();

            var today = DateTime.Today;
            return orders.Select(d =>
            {
                var ngayXuat = d.Date ?? today;
                var ngayDenHan = ngayXuat.AddDays(kh.SoNgayCongNo ?? 0);
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
                    SoNgayQuaHan = ngayDenHan < today ? (today - ngayDenHan).Days : 0
                };
            }).ToList();
        }

        public async Task<List<CongNoCustomerDTO>> GetData(CongNoFilterDTO filter)
        {
            DateTime? fromDate = null, toDate = null;
            if (DateTime.TryParse(filter?.FromDate, out var fd)) fromDate = fd;
            if (DateTime.TryParse(filter?.ToDate, out var td)) toDate = td;

            var query = _repoAccessor.KhachHang
                .FindAll(x => x.SoNgayCongNo.HasValue && x.SoNgayCongNo > 0);

            if (filter?.IdKH?.Any() == true)
                query = query.Where(x => filter.IdKH.Contains(x.ID));

            var khachHangs = await query.ToListAsync();

            var donHangQuery = _repoAccessor.DonHang.FindAll(x => x.Loai == 2);

            if (fromDate.HasValue)
                donHangQuery = donHangQuery.Where(d => d.Date >= fromDate.Value);
            if (toDate.HasValue)
                donHangQuery = donHangQuery.Where(d => d.Date <= toDate.Value.AddDays(1));

            var donHangs = await donHangQuery.ToListAsync();

            var today = DateTime.Today;
            var result = new List<CongNoCustomerDTO>();

            foreach (var kh in khachHangs)
            {
                var orders = donHangs.Where(d => d.ID_KH == kh.ID);
                var ordersWithDebt = orders.Where(d =>
                    (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0) > 0).ToList();

                if (!ordersWithDebt.Any()) continue;

                var ngayDenHanList = ordersWithDebt
                    .Where(d => d.Date.HasValue)
                    .Select(d => d.Date.Value.AddDays(kh.SoNgayCongNo ?? 0))
                    .ToList();

                var ngayDenHanGanNhat = ngayDenHanList.Any() ? ngayDenHanList.Min() : (DateTime?)null;

                var trangThai = GetTrangThai(ngayDenHanGanNhat);

                if (filter?.TrangThai == "QuaHan" && !ordersWithDebt.Any(o =>
                    o.Date.HasValue && o.Date.Value.AddDays(kh.SoNgayCongNo ?? 0) < today)) continue;
                if (filter?.TrangThai == "ConHan" && !ordersWithDebt.Any(o =>
                    o.Date.HasValue && o.Date.Value.AddDays(kh.SoNgayCongNo ?? 0) >= today)) continue;

                var detailList = ordersWithDebt.Select(d =>
                {
                    var ngayXuat = d.Date ?? today;
                    var ngayDenHan = ngayXuat.AddDays(kh.SoNgayCongNo ?? 0);
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
                        SoNgayQuaHan = ngayDenHan < today ? (today - ngayDenHan).Days : 0
                    };
                }).OrderBy(x => x.NgayDenHan).ToList();

                result.Add(new CongNoCustomerDTO
                {
                    KhachHangID = kh.ID,
                    TenKhachHang = kh.Ten,
                    SDT = kh.SDT,
                    SoNgayCongNo = kh.SoNgayCongNo ?? 0,
                    HanMucCongNo = kh.HanMucCongNo,
                    SoDonNo = ordersWithDebt.Count,
                    TongConNo = ordersWithDebt.Sum(d => (d.TongTien ?? 0) - (d.TienMat ?? 0) - (d.ChuyenKhoan ?? 0)),
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
