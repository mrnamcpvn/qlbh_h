using System.Drawing;
using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Report;
using API.Helpers.Utilities;
using API.Models;
using Aspose.Cells;
using LinqKit;
using Microsoft.EntityFrameworkCore;
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

        // public async Task<PaginationUtility<Report>> GetDataPagination(PaginationParam pagination, ReportParam param, bool isPaging = true)
        // {
        //     var data = await GetDataQuery(param);

        //     List<Report> reports = new();
        //     var group = data.GroupBy(x => new { x.MH_ID, x.CD_ID }).ToList();
        //     group.ForEach(item =>
        //     {
        //         var report = data.Where(x => x.MH_ID == item.Key.MH_ID && x.CD_ID == item.Key.CD_ID).ToList();
        //         var firstDate = report.OrderBy(x => x.Date).FirstOrDefault().Date.Day;
        //         var lastDate = report.OrderByDescending(x => x.Date).FirstOrDefault().Date.ToString("dd/MM");

        //         reports.Add(new Report
        //         {
        //             khach-hang = string.Join(", ", report.Select(x => x.khach-hang).Distinct().ToList()),
        //             CD_ID = item.Key.CD_ID,
        //             CD_Name = report.FirstOrDefault().CD_Name,
        //             MH_ID = item.Key.MH_ID,
        //             MH_Name = report.FirstOrDefault().MH_Name,
        //             Money = report.FirstOrDefault().Money,
        //             Quantity = report.Sum(x => x.Quantity),
        //             DateView = $"{firstDate} - {lastDate}"
        //         });
        //     });
        //     return PaginationUtility<Report>.Create(reports, pagination.PageNumber, pagination.PageSize, isPaging);
        // }

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

        // private async Task<List<Report>> GetDataQuery(ReportParam param)
        // {
        //     var predicate = PredicateBuilder.New<ChamCong>(x => Convert.ToDateTime(param.FromDate) <= x.Date && x.Date <= Convert.ToDateTime(param.ToDate));
        //     if (param.ID_khach-hang > 0)
        //         predicate.And(x => x.ID_khach-hang == param.ID_khach-hang);

        //     var data = await _repoAccessor.ChamCong.FindAll(predicate)
        //         .Join(_repoAccessor.NguoiLD.FindAll(),
        //             x => x.ID_khach-hang,
        //             y => y.ID,
        //             (x, y) => new { chamCong = x, khach-hang = y }
        //         ).Join(_repoAccessor.SanPham.FindAll(),
        //             x => x.chamCong.ID_CD,
        //             y => y.ID,
        //             (x, y) => new { chamCong = x.chamCong, khach-hang = x.khach-hang, SanPham = y }
        //         ).Join(_repoAccessor.MaHang.FindAll(),
        //             x => x.SanPham.IDMaHang,
        //             y => y.ID,
        //             (x, y) => new { chamCong = x.chamCong, khach-hang = x.khach-hang, SanPham = x.SanPham, maHang = y }
        //         ).Select(x => new Report
        //         {
        //             ID_khach-hang = x.chamCong.ID_khach-hang,
        //             khach-hang = x.khach-hang.Name,
        //             CD_ID = x.chamCong.ID_CD,
        //             CD_Name = x.SanPham.Name,
        //             MH_ID = x.SanPham.IDMaHang,
        //             MH_Name = x.maHang.Name,
        //             Date = x.chamCong.Date,
        //             Money = x.SanPham.Money,
        //             Quantity = x.chamCong.Quantity
        //         }).AsNoTracking().OrderBy(x => x.MH_ID).ThenBy(x => x.Date).ToListAsync();

        //     return data;
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