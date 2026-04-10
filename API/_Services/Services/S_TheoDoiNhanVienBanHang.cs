using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Helpers.Utilities;
using API.Models;
using Aspose.Cells;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_TheoDoiNhanVienBanHang : I_TheoDoiNhanVienBanHang
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_TheoDoiNhanVienBanHang(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<OperationResult> Excel(TheoDoiNhanVienBanHang_Param param)
        {
            var data = await GetData(param);
            if (!data.Any()) return new OperationResult(false, "Không có dữ liệu");
            var excel = new TheoDoiNhanVienBanHang_Data
            {
                FromDate_Str = Convert.ToDateTime(param.FromDate_Str).ToString("dd/MM/yyyy"),
                ToDate_Str = Convert.ToDateTime(param.ToDate_Str).ToString("dd/MM/yyyy"),
                Result = data,
                Tong_SL_Ban = data.Sum(x => x.SL_Ban),
                Tong_DS_Ban = data.Sum(x => x.DS_Ban),
                NVs = string.Join(", ", data.SelectMany(x => x.NV_List.Select(y => $"{y.Ten_NV} {y.SDT_NV}")).Distinct())
            };
            MemoryStream stream = new();
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"Resources\\Template\\TheoDoiNhanVienBanHang\\Download.xlsx"
            );
            WorkbookDesigner designer = new() { Workbook = new Workbook(path) };
            Worksheet ws = designer.Workbook.Worksheets[0];
            ws.Cells["A2"].PutValue($"Nhân viên : {excel.NVs}, từ ngày {excel.FromDate_Str} đến ngày {excel.ToDate_Str}");
            var rowIndex = 3;
            Style style = new CellsFactory().CreateStyle();
            style.Pattern = BackgroundType.Solid;
            AsposeUtility.SetAllBorders(style);
            foreach (var sp in excel.Result)
            {
                style.Font.IsBold = true;
                style.HorizontalAlignment = TextAlignmentType.Left;
                style.Number = 0;
                ws.Cells.Merge(rowIndex, 0, 1, 3);
                ws.Cells[rowIndex, 0].PutValue($"Tên hàng: {sp.Ten_SP} ({sp.NV_List.Count})");
                ws.Cells[rowIndex, 0].GetMergedRange().SetStyle(style);
                style.HorizontalAlignment = TextAlignmentType.Right;
                style.Custom = "#,##0";
                ws.Cells[rowIndex, 3].PutValue(sp.SL_Ban);
                ws.Cells[rowIndex, 3].SetStyle(style);
                ws.Cells[rowIndex, 4].PutValue(sp.DS_Ban);
                ws.Cells[rowIndex, 4].SetStyle(style);
                rowIndex++;
                foreach (var nv in sp.NV_List)
                {
                    style.Font.IsBold = false;
                    style.HorizontalAlignment = TextAlignmentType.Left;
                    style.Number = 0;
                    ws.Cells[rowIndex, 0].PutValue(nv.Ten_NV);
                    ws.Cells[rowIndex, 0].SetStyle(style);
                    ws.Cells[rowIndex, 1].PutValue(nv.SDT_NV);
                    ws.Cells[rowIndex, 1].SetStyle(style);
                    ws.Cells[rowIndex, 2].PutValue(nv.DVT);
                    style.HorizontalAlignment = TextAlignmentType.Right;
                    style.Custom = "#,##0";
                    ws.Cells[rowIndex, 2].SetStyle(style);
                    ws.Cells[rowIndex, 3].PutValue(nv.SL_Ban);
                    ws.Cells[rowIndex, 3].SetStyle(style);
                    ws.Cells[rowIndex, 4].PutValue(nv.DS_Ban);
                    ws.Cells[rowIndex, 4].SetStyle(style);
                    rowIndex++;
                }
            }
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.Number = 0;
            ws.Cells.Merge(rowIndex, 0, 1, 3);
            ws.Cells[rowIndex, 0].PutValue("Tổng Cộng");
            ws.Cells[rowIndex, 0].GetMergedRange().SetStyle(style);
            style.HorizontalAlignment = TextAlignmentType.Right;
            style.Custom = "#,##0";
            ws.Cells[rowIndex, 3].PutValue(excel.Tong_SL_Ban);
            ws.Cells[rowIndex, 3].SetStyle(style);
            ws.Cells[rowIndex, 4].PutValue(excel.Tong_DS_Ban);
            ws.Cells[rowIndex, 4].SetStyle(style);
            designer.Process();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);
            return new OperationResult(true, stream.ToArray());
        }

        public async Task<TheoDoiNhanVienBanHang_Data> GetDataPagination(PaginationParams pagination, TheoDoiNhanVienBanHang_Param param)
        {
            var data = await GetData(param);
            var dataPagination = PaginationUtility<TheoDoiNhanVienBanHang_SP>.Create(data, pagination.PageNumber, pagination.PageSize);
            var result = new TheoDoiNhanVienBanHang_Data
            {
                FromDate_Str = Convert.ToDateTime(param.FromDate_Str).ToString("dd/MM/yyyy"),
                ToDate_Str = Convert.ToDateTime(param.FromDate_Str).ToString("dd/MM/yyyy"),
                Pagination = dataPagination.Pagination,
                Result = dataPagination.Result,
                Tong_SL_Ban = data.Sum(x => x.SL_Ban),
                Tong_DS_Ban = data.Sum(x => x.DS_Ban),
                NVs = string.Join(", ", data.SelectMany(x => x.NV_List.Select(y => $"{y.Ten_NV} {y.SDT_NV}")).Distinct())
            };
            return result;
        }
        public async Task<List<TheoDoiNhanVienBanHang_SP>> GetData(TheoDoiNhanVienBanHang_Param param)
        {
            var predChiTietDonHang = PredicateBuilder.New<ChiTietDonHang>(true);
            var predSanPham = PredicateBuilder.New<SanPham>(true);
            var predNhanVien = PredicateBuilder.New<NhanVien>(true);
            var donHang = await _repoAccessor.DonHang.FindAll(x =>
                x.Loai.HasValue && x.Loai.Value == 2 && // Chỉ lấy đơn xuất hàng
                x.ID_NV.HasValue && param.IdNV.Contains(x.ID_NV.Value) &&
                Convert.ToDateTime(param.FromDate_Str) <= x.Date &&
                x.Date <= Convert.ToDateTime(param.ToDate_Str).AddDays(1)
            ).AsNoTracking().ToListAsync();
            var donHang_ids = donHang.Select(x => x.ID);
            var chiTiet = await _repoAccessor.ChiTietDonHang.FindAll(x =>
                donHang_ids.Contains(x.ID_DH) &&
                param.IdSP.Contains(x.ID_SP)
            ).AsNoTracking().ToListAsync();
            var nhanVien_ids = donHang.Select(x => x.ID_NV);
            var nhanVien = await _repoAccessor.NhanVien.FindAll(x =>
                nhanVien_ids.Contains(x.ID)
            ).AsNoTracking().ToListAsync();
            var sanPham_ids = chiTiet.Select(x => x.ID_SP);
            var sanPham = await _repoAccessor.SanPham.FindAll(x =>
                sanPham_ids.Contains(x.ID)
            ).AsNoTracking().ToListAsync();

            var data = donHang
                .Join(chiTiet,
                    x => x.ID,
                    y => y.ID_DH,
                    (x, y) => new { dh = x, ct = y })
                .Join(nhanVien,
                    x => x.dh.ID_NV,
                    y => y.ID,
                    (x, y) => new { x.dh, x.ct, nv = y })
                .Join(sanPham,
                    x => x.ct.ID_SP,
                    y => y.ID,
                    (x, y) => new { x.dh, x.ct, x.nv, sp = y })
                .GroupBy(x => x.sp)
                .Select(x => new TheoDoiNhanVienBanHang_SP
                {
                    Ten_SP = x.Key.Ten,
                    SL_Ban = x.Sum(y => y.ct.SoLuong),
                    DS_Ban = x.Sum(y => y.ct.ThanhTien ?? 0),
                    NV_List = x.GroupBy(y => y.nv).Select(y => new TheoDoiNhanVienBanHang_NV
                    {
                        Ten_NV = y.Key.Ten,
                        SDT_NV = y.Key.SDT,
                        DVT = x.Key.Dvt,
                        SL_Ban = y.Sum(z => z.ct.SoLuong),
                        DS_Ban = y.Sum(z => z.ct.ThanhTien ?? 0)
                    }).ToList()
                }).ToList();
            return data;
        }
        public async Task<List<KeyValuePair<int, string>>> GetListNhanVien()
        {
            var nhanVien = await _repoAccessor.NhanVien.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten}-{x.SDT}"))
                .Distinct()
                .ToListAsync();
            return nhanVien;
        }

        public async Task<List<KeyValuePair<int, string>>> GetListSanPham()
        {
            var sanPham = await _repoAccessor.SanPham.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten}"))
                .Distinct()
                .ToListAsync();
            return sanPham;
        }
    }
}