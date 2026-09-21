using System.Drawing;
using AgileObjects.AgileMapper.Extensions;
using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Report;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API._Services.Services
{
    public class S_Report : BaseServices, I_Report
    {

        public S_Report(DBContext dbContext) : base(dbContext) { }

        public async Task<OperationResult> Excel(ReportParam param)
        {
            var data = await GetDataQuery(param);
            if (!data.Result.Any()) return new OperationResult(false, "Không có dữ liệu");

            string pathFile = $"Resources\\Template\\Report\\Download.xlsx";
            List<Table> tables = new() { new("result", data.Result) };
            List<Cell> cells = new()
            {
                new Cell("A2", $"Từ ngày {Convert.ToDateTime(param.FromDate):dd/MM/yyyy} đến ngày {Convert.ToDateTime(param.ToDate):dd/MM/yyyy}"),
                new Cell($"E{5 + data.Result.Count}", data.Tong_SoLuongTonDau),
                new Cell($"F{5 + data.Result.Count}", data.Tong_GiaTon),
                new Cell($"G{5 + data.Result.Count}", data.Tong_SoLuongNhap),
                new Cell($"H{5 + data.Result.Count}", data.Tong_TongTienNhap),
                new Cell($"I{5 + data.Result.Count}", data.Tong_SoLuongXuat),
                new Cell($"J{5 + data.Result.Count}", data.Tong_TongTienXuat),
                new Cell($"K{5 + data.Result.Count}", data.Tong_SoLuongTonCuoi),
                new Cell($"L{5 + data.Result.Count}", data.Tong_DoanhThu),
            };
            ExcelResult excelResult = ExcelUtility.DownloadExcel(tables, cells, pathFile);
            return new OperationResult(excelResult.IsSuccess, excelResult.Error, excelResult.Result);
        }

        public async Task<Report_Data> GetData(ReportParam param)
        {
            var data = await GetDataQuery(param);
            return data;
        }

        private async Task<Report_Data> GetDataQuery(ReportParam param)
        {
            var fromDate = Convert.ToDateTime(param.FromDate);
            var toDate = Convert.ToDateTime(param.ToDate);

            var predicate = PredicateBuilder.New<DonHang>(x =>
                x.Date.HasValue &&
                fromDate.Date <= x.Date.Value.Date &&
                x.Date.Value.Date <= toDate.Date);
            var predicateChiTiet = PredicateBuilder.New<ChiTietDonHang>(true);
            if (param.ID_SP > 0)
                predicateChiTiet = predicateChiTiet.And(x => x.ID_SP == param.ID_SP);
            var data = await _repoAccessor.DonHang.FindAll(predicate)
                .Join(_repoAccessor.ChiTietDonHang.FindAll(predicateChiTiet),
                    x => x.ID,
                    y => y.ID_DH,
                    (x, y) => new { dh = x, ct = y })
                .Join(_repoAccessor.SanPham.FindAll(),
                    cb => cb.ct.ID_SP,
                    sp => sp.ID,
                    (cb, sp) => new { cb.dh, cb.ct, sp })
                .Select(x => new Report
                {
                    ID_SP = x.ct.ID_SP,
                    MaSP = x.sp.MaSP,
                    Ten_SP = x.sp.Ten,
                    DVT = x.sp.Dvt,
                    Gia = x.ct.Gia,
                    GiaSP = x.sp.Gia,
                    SoLuong = x.ct.SoLuong,
                    Loai = x.dh.Loai,
                    Updated_time = x.ct.Updated_Time,
                    SoLuongTrongKho = x.sp.SoLuong ?? 0,
                    CtId = x.ct.ID,
                    OrderDate = x.dh.Date ?? x.dh.Create_Time,
                    OrderCreateTime = x.dh.Create_Time,
                    OrderMa = x.dh.Ma_DH,
                }).AsNoTracking().ToListAsync();

            var spIds = data.Select(x => x.ID_SP).Distinct().ToList();
            if (spIds.Any())
            {
                var sanPhams = await _repoAccessor.SanPham
                    .FindAll(x => spIds.Contains(x.ID)).ToDictionaryAsync(x => x.ID);

                var allRecords = await _repoAccessor.ChiTietDonHang
                    .FindAll(x => spIds.Contains(x.ID_SP))
                    .Join(_repoAccessor.DonHang.FindAll(),
                        ct => ct.ID_DH, dh => dh.ID,
                        (ct, dh) => new { ct, dh })
                    .Select(x => new { x.ct.ID, x.ct.ID_SP, x.ct.SoLuong, x.dh.Loai, x.dh.Date, x.dh.Create_Time, x.dh.Ma_DH, x.ct.Updated_Time })
                    .AsNoTracking()
                    .ToListAsync();

                var lookup = allRecords.ToLookup(x => x.ID_SP);
                var dataDict = data.ToDictionary(x => x.CtId);

                foreach (var spId in spIds)
                {
                    if (!sanPhams.TryGetValue(spId, out var sp)) continue;
                    var ordered = lookup[spId]
                        .OrderBy(x => x.Date ?? DateTime.MinValue)
                        .ThenBy(x => x.Create_Time ?? DateTime.MinValue)
                        .ThenBy(x => x.Ma_DH)
                        .ThenBy(x => x.Updated_Time ?? DateTime.MinValue)
                        .ThenBy(x => x.ID)
                        .ToList();
                    int total = ordered.Sum(x => (x.Loai == 1 ? 1 : -1) * x.SoLuong);
                    int stock = (sp.SoLuong ?? 0) - total;
                    foreach (var r in ordered)
                    {
                        if (dataDict.TryGetValue(r.ID, out var item))
                            item.SLTonDau = stock;
                        stock += (r.Loai == 1 ? 1 : -1) * r.SoLuong;
                        if (dataDict.TryGetValue(r.ID, out var item2))
                            item2.SLTonCuoi = stock;
                    }
                }
            }

            List<ReportDTO> reports = data.GroupBy(x => x.ID_SP)
                .OrderBy(g => g.Key)
                .Select((g, i) =>
                {
                    // Sắp theo đúng thứ tự tính SL_Ton trong S_DonHang để lấy đúng tồn đầu/cuối kỳ
                    var ordered = g
                        .OrderBy(x => x.OrderDate ?? DateTime.MinValue)
                        .ThenBy(x => x.OrderCreateTime ?? DateTime.MinValue)
                        .ThenBy(x => x.OrderMa)
                        .ThenBy(x => x.Updated_time ?? DateTime.MinValue)
                        .ThenBy(x => x.CtId)
                        .ToList();
                    var firstItem = ordered.FirstOrDefault();
                    var lastInbound = ordered.LastOrDefault(x => x.Loai == 1);
                    var itemRP = new ReportDTO
                    {
                        Stt = i + 1,
                        ID_SP = g.Key,
                        MaSP = firstItem?.MaSP ?? "",
                        Ten_SP = firstItem?.Ten_SP ?? "",
                        DVT = firstItem?.DVT ?? "",
                        GiaTon = lastInbound?.Gia ?? firstItem?.GiaSP,
                        SoLuongNhap = g.Where(x => x.Loai == 1).Sum(x => x.SoLuong),
                        SoLuongXuat = g.Where(x => x.Loai == 2).Sum(x => x.SoLuong),
                        TongTienNhap = g.Where(x => x.Loai == 1).Sum(x => x.SoLuong * x.Gia),
                        TongTienXuat = g.Where(x => x.Loai == 2).Sum(x => x.SoLuong * x.Gia),
                        SoLuongTonDau = ordered.First().SLTonDau ?? 0,
                        SoLuongTonCuoi = ordered.Last().SLTonCuoi ?? 0
                    };
                    itemRP.DoanhThu = (itemRP.TongTienXuat ?? 0) - (itemRP.TongTienNhap ?? 0);
                    return itemRP;
                }).ToList();

            var resutl = new Report_Data
            {
                Result = reports,
                Tong_SoLuongTonDau = reports.Sum(z => z.SoLuongTonDau ?? 0),
                Tong_GiaTon = reports.Sum(z => z.GiaTon ?? 0),
                Tong_SoLuongNhap = reports.Sum(z => z.SoLuongNhap),
                Tong_TongTienNhap = reports.Sum(z => z.TongTienNhap ?? 0),
                Tong_SoLuongXuat = reports.Sum(z => z.SoLuongXuat),
                Tong_TongTienXuat = reports.Sum(z => z.TongTienXuat ?? 0),
                Tong_SoLuongTonCuoi = reports.Sum(z => z.SoLuongTonCuoi ?? 0),
                Tong_DoanhThu = reports.Sum(z => z.DoanhThu ?? 0)
            };
            return resutl;
        }

    }
}