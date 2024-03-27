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
            group.ForEach(item =>
            {
                var listDetail = item.ToList();
                var itemRP = new ReportDTO();
                itemRP.ID_SP = item.Key.ID_SP;
                itemRP.Ten_SP = item.FirstOrDefault().Ten_SP;
                itemRP.GiaTon = item.OrderByDescending(x=> x.Updated_time).FirstOrDefault(x => x.Loai == 1).Gia;
                itemRP.SoLuongNhap = item.Filter(x=>x.Loai == 1).Sum(x=> x.SoLuong);
                itemRP.SoLuongXuat = item.Filter(x=>x.Loai == 2).Sum(x=> x.SoLuong);
                itemRP.TongTienNhap = item.Filter(x=>x.Loai == 1).Sum(x=> x.SoLuong*x.Gia);
                itemRP.TongTienXuat = item.Filter(x=>x.Loai == 2).Sum(x=> x.SoLuong*x.Gia);
                itemRP.SoLuongTonDau = item.OrderBy(x=> x.Updated_time).FirstOrDefault().SLTonDau;
                itemRP.SoLuongTonCuoi = item.OrderByDescending(x=> x.Updated_time).FirstOrDefault().SLTonCuoi;
                itemRP.DoanhThu = itemRP.SoLuongTonDau* itemRP.GiaTon + itemRP.TongTienNhap - itemRP.TongTienXuat;
                
                reports.Add(itemRP);
            });
            return PaginationUtility<ReportDTO>.Create(reports, pagination.PageNumber, pagination.PageSize, isPaging);
        }

        private async Task<List<Report>> GetDataQuery(ReportParam param)
        {
            var predicate = PredicateBuilder.New<DonHang>(x => Convert.ToDateTime(param.FromDate) <= x.Date && x.Date <= Convert.ToDateTime(param.ToDate));
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

        // public async Task<byte[]> ExportExcel(PaginationParam pagination, ReportParam param, bool isPaging = true)
        // {
        //     var data = await GetDataQuery(param);

        //     var group = data.GroupBy(x => x.ID_khach-hang).OrderBy(x => x.Key).ToList();

        //     var template = Path.Combine(Directory.GetCurrentDirectory(), "Resources/Template/ReportMain.xlsx");
        //     Workbook workbook = new Workbook(template);
        //     WorkbookDesigner designer = new WorkbookDesigner(workbook);

        //     var now = DateTime.Now;

        //     Style styleCommodityCode = designer.Workbook.CreateStyle();
        //     styleCommodityCode = GetStyleCommodityCode(styleCommodityCode);

        //     Style styleThead = designer.Workbook.CreateStyle();
        //     styleThead = GetStyleThead(styleThead);

        //     StyleFlag flg = new StyleFlag();
        //     flg.All = true;

        //     foreach (var item in group)
        //     {
        //         int index = 1;
        //         // Add new sheet and set Name sheet
        //         int sheet = workbook.Worksheets.Add();
        //         Worksheet ws = workbook.Worksheets[sheet];
        //         ws.Copy(workbook.Worksheets[0]);
        //         ws.Name = data.FirstOrDefault(x => x.ID_khach-hang == item.Key).khach-hang;

        //         ws.Cells.Merge(index - 1, 0, 1, 5);

        //         ws.Cells["A" + index].PutValue($"Họ và tên: {ws.Name}");
        //         Style styleName = ws.Cells["A" + index].GetStyle();
        //         styleName.VerticalAlignment = TextAlignmentType.Center;
        //         styleName.HorizontalAlignment = TextAlignmentType.Center;
        //         styleName.Font.IsBold = true;
        //         styleName.Font.Size = 24;
        //         ws.Cells["A" + index].SetStyle(styleName);

        //         index += 2;

        //         var dataDetails = data.Where(x => x.ID_khach-hang == item.Key).ToList();
        //         var groupDetails = dataDetails.GroupBy(x => x.MH_ID.Value).ToList();

        //         foreach (var gd in groupDetails)
        //         {
        //             Aspose.Cells.Range range = ws.Cells.CreateRange(index - 1, 0, 1, 5);
        //             range.ApplyStyle(styleCommodityCode, flg);

        //             ws.Cells.Merge(index - 1, 0, 1, 5);

        //             ws.Cells["A" + index].PutValue($"Mã hàng: {gd.FirstOrDefault(x => x.MH_ID == gd.Key).MH_Name}");

        //             int row = index;

        //             index++;

        //             var details = dataDetails.Where(x => x.MH_ID == gd.Key).ToList();

        //             ws.Cells["A" + index].PutValue("STT");
        //             ws.Cells["B" + index].PutValue("Ngày");
        //             ws.Cells["C" + index].PutValue("Công đoạn");
        //             ws.Cells["D" + index].PutValue("Số lượng");
        //             ws.Cells["E" + index].PutValue("Thành tiền");

        //             Aspose.Cells.Range rangeThead = ws.Cells.CreateRange(index - 1, 0, 1, 5);
        //             rangeThead.ApplyStyle(styleThead, flg);

        //             index++;

        //             int stt = 1;
        //             details.ForEach(x =>
        //             {
        //                 ws.Cells["A" + index].PutValue(stt);
        //                 ws.Cells["B" + index].PutValue(x.Date.ToString("dd/MM/yyyy"));
        //                 ws.Cells["C" + index].PutValue(x.CD_Name);
        //                 ws.Cells["D" + index].PutValue(x.Quantity);
        //                 ws.Cells["E" + index].PutValue(x.Total_Money_View);

        //                 stt++;
        //                 index++;
        //             });

        //             Aspose.Cells.Range rangeDetails = ws.Cells.CreateRange(row + 1, 0, index - row - 1, 5);
        //             rangeDetails.SetOutlineBorders(CellBorderType.Thin, Color.Black);

        //             ws.Cells.Merge(index - 1, 0, 1, 4);

        //             ws.Cells["A" + index].PutValue("Tổng tiền");
        //             Style styleA = ws.Cells["A" + index].GetStyle();
        //             styleA.VerticalAlignment = TextAlignmentType.Center;
        //             styleA.HorizontalAlignment = TextAlignmentType.Center;
        //             styleA.ForegroundColor = Color.Yellow;
        //             styleA.Pattern = BackgroundType.Solid;
        //             styleA.Font.IsBold = true;
        //             ws.Cells["A" + index].SetStyle(styleA);

        //             ws.Cells["E" + index].PutValue((details.Sum(x => x.Total_Money)).ToString("#,##"));
        //             Style styleE = ws.Cells["E" + index].GetStyle();
        //             styleE.VerticalAlignment = TextAlignmentType.Center;
        //             styleE.HorizontalAlignment = TextAlignmentType.Right;
        //             styleE.ForegroundColor = Color.Yellow;
        //             styleE.Pattern = BackgroundType.Solid;
        //             styleE.Font.IsBold = true;
        //             ws.Cells["E" + index].SetStyle(styleE);

        //             index += 2;
        //         }

        //         designer.Workbook = workbook;
        //     }
        //     designer.Workbook.Worksheets.RemoveAt(0);
        //     designer.Workbook.Worksheets.ActiveSheetIndex = 0;
        //     MemoryStream stream = new MemoryStream();
        //     if (data.Any())
        //     {
        //         designer.Workbook.Save(stream, SaveFormat.Xlsx);
        //     }
        //     return stream.ToArray();
        // }

        

        private Style GetStyleCommodityCode(Style style)
        {
            style = GetStyle(style);
            style.ForegroundColor = Color.FromArgb(112, 173, 71);
            style.Font.Size = 16;
            return style;
        }

        private Style GetStyleThead(Style style)
        {
            style = GetStyle(style);
            style.ForegroundColor = Color.FromArgb(155, 194, 230);
            return style;
        }

        private Style GetStyle(Style style)
        {
            style.SetAllBorders();
            style.VerticalAlignment = TextAlignmentType.Center;
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.Pattern = BackgroundType.Solid;
            style.Font.IsBold = true;
            return style;
        }
    }
}