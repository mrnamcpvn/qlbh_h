using System.Drawing;
using AgileObjects.AgileMapper.Extensions;
using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Report;
using API.Helpers.Utilities;
using API.Models;
using Aspose.Cells;
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

        public async Task<PaginationUtility<ReportDTO>> GetDataPagination(PaginationParam pagination, ReportParam param, bool isPaging = true)
        {
            var data = await GetDataQuery(param);
            if(param.ID_SP > 0) {
                data = data.Where(x=> x.ID_SP == param.ID_SP).ToList();
            }
            List<ReportDTO> reports = new();
            var group = data.GroupBy(x => new { x.ID_SP }).ToList();
            group.ForEach(async item =>
            {
                var itemRP = new ReportDTO();
                itemRP.ID_SP = item.Key.ID_SP;
                itemRP.Ten_SP = item.FirstOrDefault().Ten_SP;
                var t = item.OrderByDescending(x=> x.Updated_time).FirstOrDefault(x => x.Loai == 1);
                itemRP.GiaTon = t != null ? t.Gia : 0;
                itemRP.SoLuongNhap = item.Filter(x=>x.Loai == 1).Sum(x=> x.SoLuong);
                itemRP.SoLuongXuat = item.Filter(x=>x.Loai == 2).Sum(x=> x.SoLuong);
                itemRP.TongTienNhap = item.Filter(x=>x.Loai == 1).Sum(x=> x.SoLuong*x.Gia);
                itemRP.TongTienXuat = item.Filter(x=>x.Loai == 2).Sum(x=> x.SoLuong*x.Gia);
                itemRP.SoLuongTonDau = item.OrderBy(x=> x.Updated_time).FirstOrDefault().SLTonDau??0;
                var sp =  _repoAccessor.SanPham.FindAll(x=> x.ID == itemRP.ID_SP).FirstOrDefault();
                itemRP.SoLuongTonCuoi = sp.SoLuong;
                itemRP.DoanhThu = itemRP.TongTienXuat - itemRP.TongTienNhap;
                
                reports.Add(itemRP);
            });
            return PaginationUtility<ReportDTO>.Create(reports, pagination.PageNumber, pagination.PageSize, isPaging);
        }

        private async Task<List<Report>> GetDataQuery(ReportParam param)
        {
            var predicate = PredicateBuilder.New<DonHang>(x => Convert.ToDateTime(param.FromDate) <= x.Date && x.Date <= Convert.ToDateTime(param.ToDate).AddDays(1));
            var data = await _repoAccessor.DonHang.FindAll(predicate)
                .Join(_repoAccessor.ChiTietDonHang.FindAll(),
                    x => x.ID,
                    y => y.ID_DH,
                    (x, y) => new { donHang = x, chiTiet = y }
                ).Select(x => new Report
                {
                    ID_SP = x.chiTiet.ID_SP,
                    Ten_SP = x.chiTiet.Ten_SP,
                    Gia = x.chiTiet.Gia,
                    SoLuong = x.chiTiet.SoLuong,
                    SLTonDau = x.chiTiet.SL_Ton_Dau,
                    SLTonCuoi = x.chiTiet.SL_Ton_Cuoi,
                    Loai = x.donHang.Loai,
                    Updated_time = x.chiTiet.Updated_Time
                }).AsNoTracking().OrderByDescending(x => x.ID_SP).ToListAsync();

            return data;
        }

    }
}