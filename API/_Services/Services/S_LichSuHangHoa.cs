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
    public class S_LichSuHangHoa : I_LichSuHangHoa
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_LichSuHangHoa(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<LichSuHangHoaData> GetDataPagination(PaginationParams pagination, LichSuHangHoaParam param)
        {
            var allData = await GetAllData(param);
            var dataPagination = PaginationUtility<LichSuHangHoaItem>.Create(allData, pagination.PageNumber, pagination.PageSize);
            return new LichSuHangHoaData
            {
                Pagination = dataPagination.Pagination,
                Result = dataPagination.Result,
                TongSLNhap = allData.Where(x => x.Loai == 1).Sum(x => x.SoLuong),
                TongSLXuat = allData.Where(x => x.Loai == 2).Sum(x => x.SoLuong),
                TongTienNhap = allData.Where(x => x.Loai == 1).Sum(x => x.ThanhTien),
                TongTienXuat = allData.Where(x => x.Loai == 2).Sum(x => x.ThanhTien)
            };
        }

        public async Task<OperationResult> Excel(LichSuHangHoaParam param)
        {
            var fromDate = Convert.ToDateTime(param.FromDate);
            var toDate = Convert.ToDateTime(param.ToDate);
            var allData = await GetAllData(param);

            if (!allData.Any()) return new OperationResult(false, "Không có dữ liệu");

            MemoryStream stream = new();
            Workbook workbook = new();
            Worksheet ws = workbook.Worksheets[0];
            ws.Name = "LichSuHangHoa";

            Style style = new CellsFactory().CreateStyle();
            style.Font.Name = "Calibri";
            style.Font.Size = 16;
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;

            ws.Cells.Merge(0, 0, 1, 11);
            ws.Cells[0, 0].PutValue("LỊCH SỬ HÀNG HÓA");
            ws.Cells[0, 0].GetMergedRange().SetStyle(style);

            style.Font.Size = 11;
            style.HorizontalAlignment = TextAlignmentType.Center;
            ws.Cells.Merge(1, 0, 1, 11);
            ws.Cells[1, 0].PutValue($"Từ ngày {fromDate:dd/MM/yyyy} đến ngày {toDate:dd/MM/yyyy}");
            ws.Cells[1, 0].GetMergedRange().SetStyle(style);

            var headers = new[] { "Ngày", "Loại", "Mã ĐH", "Đối tác", "NV", "Sản Phẩm", "SL", "Giá", "Thành tiền", "Tồn đầu", "Tồn cuối" };
            style.Font.IsBold = true;
            style.Pattern = BackgroundType.Solid;
            style.Font.Color = System.Drawing.Color.Black;
            style.ForegroundColor = System.Drawing.Color.FromArgb(170, 225, 230);
            style.HorizontalAlignment = TextAlignmentType.Center;
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[2, i].PutValue(headers[i]);
                ws.Cells[2, i].SetStyle(style);
            }

            var rowIndex = 3;
            style.Font.IsBold = false;
            style.Pattern = BackgroundType.Solid;
            style.Font.Color = System.Drawing.Color.Black;
            style.ForegroundColor = System.Drawing.Color.White;
            style.HorizontalAlignment = TextAlignmentType.Left;
            foreach (var item in allData)
            {
                ws.Cells[rowIndex, 0].PutValue(item.Date?.ToString("dd/MM/yyyy"));
                ws.Cells[rowIndex, 0].SetStyle(style);
                ws.Cells[rowIndex, 1].PutValue(item.LoaiStr);
                style.HorizontalAlignment = TextAlignmentType.Center;
                ws.Cells[rowIndex, 1].SetStyle(style);
                ws.Cells[rowIndex, 2].PutValue(item.Ma_DH);
                ws.Cells[rowIndex, 2].SetStyle(style);
                style.HorizontalAlignment = TextAlignmentType.Left;
                ws.Cells[rowIndex, 3].PutValue(item.DoiTac);
                ws.Cells[rowIndex, 3].SetStyle(style);
                ws.Cells[rowIndex, 4].PutValue(item.Ten_NV);
                ws.Cells[rowIndex, 4].SetStyle(style);
                ws.Cells[rowIndex, 5].PutValue(item.Ten_SP);
                ws.Cells[rowIndex, 5].SetStyle(style);
                style.HorizontalAlignment = TextAlignmentType.Center;
                ws.Cells[rowIndex, 6].PutValue(item.SoLuong);
                ws.Cells[rowIndex, 6].SetStyle(style);
                style.Custom = "#,##0";
                style.HorizontalAlignment = TextAlignmentType.Right;
                ws.Cells[rowIndex, 7].PutValue(item.Gia);
                ws.Cells[rowIndex, 7].SetStyle(style);
                ws.Cells[rowIndex, 8].PutValue(item.ThanhTien);
                ws.Cells[rowIndex, 8].SetStyle(style);
                style.Custom = null;
                style.HorizontalAlignment = TextAlignmentType.Center;
                ws.Cells[rowIndex, 9].PutValue(item.SL_Ton_Dau);
                ws.Cells[rowIndex, 9].SetStyle(style);
                ws.Cells[rowIndex, 10].PutValue(item.SL_Ton_Cuoi);
                ws.Cells[rowIndex, 10].SetStyle(style);
                rowIndex++;
            }

            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Right;
            style.Custom = "#,##0";
            ws.Cells.Merge(rowIndex, 0, 1, 6);
            ws.Cells[rowIndex, 0].PutValue("Tổng Cộng");
            style.HorizontalAlignment = TextAlignmentType.Center;
            ws.Cells[rowIndex, 0].GetMergedRange().SetStyle(style);
            style.HorizontalAlignment = TextAlignmentType.Right;
            ws.Cells[rowIndex, 6].PutValue($"N:{allData.Where(x => x.Loai == 1).Sum(x => x.SoLuong)}/X:{allData.Where(x => x.Loai == 2).Sum(x => x.SoLuong)}");
            ws.Cells[rowIndex, 6].SetStyle(style);
            ws.Cells[rowIndex, 8].PutValue($"N:{allData.Where(x => x.Loai == 1).Sum(x => x.ThanhTien):#,##0}/X:{allData.Where(x => x.Loai == 2).Sum(x => x.ThanhTien):#,##0}");
            ws.Cells[rowIndex, 8].SetStyle(style);
            ws.Cells[rowIndex, 7].PutValue("");
            ws.Cells[rowIndex, 7].SetStyle(style);
            ws.Cells[rowIndex, 9].PutValue("");
            ws.Cells[rowIndex, 9].SetStyle(style);
            ws.Cells[rowIndex, 10].PutValue("");
            ws.Cells[rowIndex, 10].SetStyle(style);

            var dataRange = ws.Cells.CreateRange(2, 0, rowIndex - 1, 11);
            var borderStyle = new CellsFactory().CreateStyle();
            borderStyle.SetAllBorders();
            dataRange.ApplyStyle(borderStyle, new StyleFlag { Borders = true });

            ws.AutoFitColumns();
            workbook.Save(stream, SaveFormat.Xlsx);
            return new OperationResult(true, stream.ToArray());
        }

        private async Task<List<LichSuHangHoaItem>> GetAllData(LichSuHangHoaParam param)
        {
            var fromDate = Convert.ToDateTime(param.FromDate);
            var toDate = Convert.ToDateTime(param.ToDate);

            var predicate = PredicateBuilder.New<ChiTietDonHang>(true);

            if (param.IdSP != null && param.IdSP.Count > 0)
            {
                predicate = predicate.And(x => param.IdSP.Contains(x.ID_SP));
            }

            var donHangPredicate = PredicateBuilder.New<DonHang>(x =>
                x.Date.HasValue &&
                fromDate.Date <= x.Date.Value.Date &&
                x.Date.Value.Date <= toDate.Date
            );

            if (param.Loai.HasValue && param.Loai.Value > 0)
            {
                donHangPredicate = donHangPredicate.And(x => x.Loai == param.Loai.Value);
            }

            var hasKH = param.IdKH != null && param.IdKH.Count > 0;
            var hasNCC = param.IdNCC != null && param.IdNCC.Count > 0;

            if (hasKH && hasNCC)
            {
                donHangPredicate = donHangPredicate.And(x =>
                    (x.ID_KH.HasValue && param.IdKH.Contains(x.ID_KH.Value)) ||
                    (x.ID_NCC.HasValue && param.IdNCC.Contains(x.ID_NCC.Value)));
            }
            else
            {
                if (hasKH)
                    donHangPredicate = donHangPredicate.And(x =>
                        x.ID_KH.HasValue && param.IdKH.Contains(x.ID_KH.Value));
                if (hasNCC)
                    donHangPredicate = donHangPredicate.And(x =>
                        x.ID_NCC.HasValue && param.IdNCC.Contains(x.ID_NCC.Value));
            }

            if (param.IdNV != null && param.IdNV.Count > 0)
            {
                donHangPredicate = donHangPredicate.And(x =>
                    x.ID_NV.HasValue && param.IdNV.Contains(x.ID_NV.Value));
            }

            var donHangQuery = _repoAccessor.DonHang.FindAll(donHangPredicate).AsNoTracking();
            var chiTietQuery = _repoAccessor.ChiTietDonHang.FindAll(predicate).AsNoTracking();
            var sanPhamQuery = _repoAccessor.SanPham.FindAll().AsNoTracking();
            var khachHangQuery = _repoAccessor.KhachHang.FindAll().AsNoTracking();
            var nhaCungCapQuery = _repoAccessor.NhaCungCap.FindAll().AsNoTracking();
            var nhanVienQuery = _repoAccessor.NhanVien.FindAll().AsNoTracking();

            var query = chiTietQuery
                .Join(donHangQuery, ct => ct.ID_DH, dh => dh.ID, (ct, dh) => new { ct, dh })
                .Join(sanPhamQuery, x => x.ct.ID_SP, sp => sp.ID, (x, sp) => new { x.ct, x.dh, sp })
                .GroupJoin(nhanVienQuery, x => x.dh.ID_NV, nv => nv.ID, (x, nvJoin) => new { x.ct, x.dh, x.sp, nvJoin })
                .SelectMany(x => x.nvJoin.DefaultIfEmpty(), (x, nv) => new { x.ct, x.dh, x.sp, nv })
                .GroupJoin(khachHangQuery, x => x.dh.ID_KH, kh => kh.ID, (x, khJoin) => new { x.ct, x.dh, x.sp, x.nv, khJoin })
                .SelectMany(x => x.khJoin.DefaultIfEmpty(), (x, kh) => new { x.ct, x.dh, x.sp, x.nv, kh })
                .GroupJoin(nhaCungCapQuery, x => x.dh.ID_NCC, ncc => ncc.ID, (x, nccJoin) => new { x.ct, x.dh, x.sp, x.nv, x.kh, nccJoin })
                .SelectMany(x => x.nccJoin.DefaultIfEmpty(), (x, ncc) => new { x.ct, x.dh, x.sp, x.nv, x.kh, ncc })
                .Select(x => new LichSuHangHoaItem
                {
                    ID = x.ct.ID,
                    ID_SP = x.ct.ID_SP,
                    MaSP = x.sp.MaSP,
                    Ten_SP = x.sp.Ten,
                    Dvt = x.sp.Dvt,
                    ID_DH = x.dh.ID,
                    Ma_DH = x.dh.Ma_DH,
                    Loai = x.dh.Loai ?? 0,
                    LoaiStr = x.dh.Loai == 1 ? "Nhập" : "Xuất",
                    DoiTac = x.dh.Loai == 1 ? x.ncc.Ten : x.kh.Ten,
                    Ten_NV = x.nv.Ten,
                    SoLuong = x.ct.SoLuong,
                    Gia = x.ct.Gia ?? 0,
                    ThanhTien = x.ct.ThanhTien ?? 0,
                    SL_Ton_Dau = x.ct.SL_Ton_Dau,
                    SL_Ton_Cuoi = x.ct.SL_Ton_Cuoi,
                    Date = x.dh.Date,
                    Updated_Time = x.ct.Updated_Time
                });

            return await query.OrderBy(x => x.Date).ThenBy(x => x.Updated_Time).ToListAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListSanPham()
        {
            return await _repoAccessor.SanPham.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.MaSP})"))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListKhachHang()
        {
            return await _repoAccessor.KhachHang.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.Ma_KH})"))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListNhaCungCap()
        {
            return await _repoAccessor.NhaCungCap.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.Ma_NCC})"))
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListNhanVien()
        {
            return await _repoAccessor.NhanVien.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, x.Ten))
                .Distinct()
                .ToListAsync();
        }
    }
}
