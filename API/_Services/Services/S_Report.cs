using System.Drawing;
using AgileObjects.AgileMapper.Extensions;
using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Report;
using API.Helper.Utilities;
using API.Helpers.Utilities;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_Report : I_Report
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_Report(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<OperationResult> Excel(ReportParam param)
        {
            var data = await GetDataQuery(param);
            if (!data.Result.Any()) return new OperationResult(false, "Không có dữ liệu");

            string pathFile = $"Resources\\Template\\Report\\Download.xlsx";
            List<Table> tables = new() { new("result", data.Result) };
            List<Cell> cells = new()
            {
                new Cell("A2", $"Từ ngày {Convert.ToDateTime(param.FromDate):dd/MM/yyyy} đến ngày {Convert.ToDateTime(param.ToDate):dd/MM/yyyy}"),
                new Cell($"D{5 + data.Result.Count}", data.Tong_SoLuongTonDau),
                new Cell($"E{5 + data.Result.Count}", data.Tong_GiaTon),
                new Cell($"F{5 + data.Result.Count}", data.Tong_SoLuongNhap),
                new Cell($"G{5 + data.Result.Count}", data.Tong_TongTienNhap),
                new Cell($"H{5 + data.Result.Count}", data.Tong_SoLuongXuat),
                new Cell($"I{5 + data.Result.Count}", data.Tong_TongTienXuat),
                new Cell($"J{5 + data.Result.Count}", data.Tong_SoLuongTonCuoi),
                new Cell($"K{5 + data.Result.Count}", data.Tong_DoanhThu),
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
            var predicate = PredicateBuilder.New<DonHang>(x => Convert.ToDateTime(param.FromDate) <= x.Date && x.Date <= Convert.ToDateTime(param.ToDate).AddDays(1));
            var predicateChiTiet = PredicateBuilder.New<ChiTietDonHang>(true);
            if (param.ID_SP > 0)
                predicateChiTiet.And(x => x.ID_SP == param.ID_SP);
            var data = await _repoAccessor.DonHang.FindAll(predicate)
                .Join(_repoAccessor.ChiTietDonHang.FindAll(predicateChiTiet),
                    x => x.ID,
                    y => y.ID_DH,
                    (x, y) => new { donHang = x, chiTiet = y }
                ).Select(x => new Report
                {
                    ID_SP = x.chiTiet.ID_SP,
                    Ten_SP = x.chiTiet.Ten_SP,
                    DVT = x.chiTiet.Dvt,
                    Gia = x.chiTiet.Gia,
                    SoLuong = x.chiTiet.SoLuong,
                    SLTonDau = x.chiTiet.SL_Ton_Dau,
                    SLTonCuoi = x.chiTiet.SL_Ton_Cuoi,
                    Loai = x.donHang.Loai,
                    Updated_time = x.chiTiet.Updated_Time
                }).AsNoTracking().OrderByDescending(x => x.ID_SP).ToListAsync();
            List<ReportDTO> reports = data.GroupBy(x => x.ID_SP)
                .Select((item, i) =>
                {
                    var t = item.OrderByDescending(x => x.Updated_time).FirstOrDefault(x => x.Loai == 1);
                    var sp = _repoAccessor.SanPham.FindAll(x => x.ID == item.Key).FirstOrDefault();
                    var itemRP = new ReportDTO
                    {
                        Stt = i + 1,
                        ID_SP = item.Key,
                        Ten_SP = item.FirstOrDefault().Ten_SP,
                        DVT = item.FirstOrDefault().DVT,
                        GiaTon = t != null ? t.Gia : 0,
                        SoLuongNhap = item.Filter(x => x.Loai == 1).Sum(x => x.SoLuong),
                        SoLuongXuat = item.Filter(x => x.Loai == 2).Sum(x => x.SoLuong),
                        TongTienNhap = item.Filter(x => x.Loai == 1).Sum(x => x.SoLuong * x.Gia),
                        TongTienXuat = item.Filter(x => x.Loai == 2).Sum(x => x.SoLuong * x.Gia),
                        SoLuongTonDau = item.OrderBy(x => x.Updated_time).FirstOrDefault().SLTonDau ?? 0,
                        SoLuongTonCuoi = sp.SoLuong
                    };
                    itemRP.DoanhThu = itemRP.TongTienXuat - itemRP.TongTienNhap;
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