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
            var nvs = string.Join(", ", data.Select(x => $"{x.Ten_NV} {x.SDT_NV}"));
            var fromDate = Convert.ToDateTime(param.FromDate_Str).ToString("dd/MM/yyyy");
            var toDate = Convert.ToDateTime(param.ToDate_Str).ToString("dd/MM/yyyy");

            MemoryStream stream = new();
            Workbook workbook = new();
            Worksheet ws = workbook.Worksheets[0];
            ws.Name = "TheoDoiNhanVienBanHang";

            Style titleStyle = new CellsFactory().CreateStyle();
            titleStyle.Font.Name = "Calibri";
            titleStyle.Font.Size = 16;
            titleStyle.Font.IsBold = true;
            titleStyle.HorizontalAlignment = TextAlignmentType.Center;
            titleStyle.VerticalAlignment = TextAlignmentType.Center;

            ws.Cells.Merge(0, 0, 1, 9);
            ws.Cells[0, 0].PutValue("BÁO CÁO THEO DÕI NHÂN VIÊN BÁN HÀNG");
            ws.Cells[0, 0].GetMergedRange().SetStyle(titleStyle);

            Style infoStyle = new CellsFactory().CreateStyle();
            infoStyle.Font.Size = 11;
            infoStyle.Font.IsBold = true;
            infoStyle.HorizontalAlignment = TextAlignmentType.Center;
            ws.Cells.Merge(1, 0, 1, 9);
            ws.Cells[1, 0].PutValue($"Nhân viên: {nvs}, từ ngày {fromDate} đến ngày {toDate}");
            ws.Cells[1, 0].GetMergedRange().SetStyle(infoStyle);

            var headers = new[] { "Nhân Viên", "Đơn Hàng", "Sản Phẩm", "Đvt", "SL", "Thành Tiền", "Đã Thu", "Còn Nợ", "Trạng Thái" };
            Style headerStyle = new CellsFactory().CreateStyle();
            headerStyle.SetAllBorders();
            headerStyle.Font.IsBold = true;
            headerStyle.Pattern = BackgroundType.Solid;
            headerStyle.Font.Color = System.Drawing.Color.Black;
            headerStyle.ForegroundColor = System.Drawing.Color.FromArgb(170, 225, 230);
            headerStyle.HorizontalAlignment = TextAlignmentType.Center;
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[2, i].PutValue(headers[i]);
                ws.Cells[2, i].SetStyle(headerStyle);
            }

            var rowIndex = 3;
            var nvStyle = new CellsFactory().CreateStyle();
            nvStyle.SetAllBorders();
            nvStyle.Font.IsBold = true;
            nvStyle.Pattern = BackgroundType.Solid;
            nvStyle.ForegroundColor = System.Drawing.Color.FromArgb(220, 220, 220);
            var orderStyle = new CellsFactory().CreateStyle();
            orderStyle.SetAllBorders();
            orderStyle.Pattern = BackgroundType.Solid;
            orderStyle.ForegroundColor = System.Drawing.Color.FromArgb(241, 243, 245);
            var productStyle = new CellsFactory().CreateStyle();
            productStyle.SetAllBorders();
            var nvNumStyle = new CellsFactory().CreateStyle();
            nvNumStyle.SetAllBorders();
            nvNumStyle.Font.IsBold = true;
            nvNumStyle.Pattern = BackgroundType.Solid;
            nvNumStyle.ForegroundColor = System.Drawing.Color.FromArgb(220, 220, 220);
            nvNumStyle.HorizontalAlignment = TextAlignmentType.Right;
            nvNumStyle.Custom = "#,##0";
            var orderNumStyle = new CellsFactory().CreateStyle();
            orderNumStyle.SetAllBorders();
            orderNumStyle.Pattern = BackgroundType.Solid;
            orderNumStyle.ForegroundColor = System.Drawing.Color.FromArgb(241, 243, 245);
            orderNumStyle.HorizontalAlignment = TextAlignmentType.Right;
            orderNumStyle.Custom = "#,##0";
            var productNumStyle = new CellsFactory().CreateStyle();
            productNumStyle.SetAllBorders();
            productNumStyle.HorizontalAlignment = TextAlignmentType.Right;
            productNumStyle.Custom = "#,##0";

            foreach (var nv in data)
            {
                ws.Cells[rowIndex, 0].PutValue($"NV: {nv.Ten_NV} ({nv.SDT_NV})");
                ws.Cells[rowIndex, 0].SetStyle(nvStyle);
                ws.Cells[rowIndex, 1].PutValue($"{nv.So_Don} đơn");
                ws.Cells[rowIndex, 1].SetStyle(nvStyle);
                ws.Cells.Merge(rowIndex, 2, 1, 3);
                ws.Cells[rowIndex, 2].PutValue($"{nv.SoLoaiSP} sản phẩm");
                ws.Cells[rowIndex, 2].GetMergedRange().SetStyle(nvStyle);
                ws.Cells[rowIndex, 5].PutValue(nv.DS_Ban);
                ws.Cells[rowIndex, 5].SetStyle(nvNumStyle);
                ws.Cells[rowIndex, 6].PutValue(nv.DaThu);
                ws.Cells[rowIndex, 6].SetStyle(nvNumStyle);
                ws.Cells[rowIndex, 7].PutValue(nv.CongNo);
                ws.Cells[rowIndex, 7].SetStyle(nvNumStyle);
                ws.Cells[rowIndex, 8].PutValue($"Đã TT: {nv.SoDon_DaThanhToan} | Chưa TT: {nv.SoDon_ChuaThanhToan}");
                ws.Cells[rowIndex, 8].SetStyle(nvStyle);
                rowIndex++;

                foreach (var dh in nv.DonHang_List)
                {
                    ws.Cells[rowIndex, 0].SetStyle(orderStyle);
                    ws.Cells[rowIndex, 1].PutValue($"{dh.Ma_DH} | Ngày xuất: {dh.Date?.ToString("dd/MM/yyyy")}");
                    ws.Cells[rowIndex, 1].SetStyle(orderStyle);
                    ws.Cells.Merge(rowIndex, 2, 1, 3);
                    ws.Cells[rowIndex, 2].PutValue($"{dh.SoLoaiSP} sản phẩm");
                    ws.Cells[rowIndex, 2].GetMergedRange().SetStyle(orderStyle);
                    ws.Cells[rowIndex, 5].PutValue(dh.TongTien);
                    ws.Cells[rowIndex, 5].SetStyle(orderNumStyle);
                    ws.Cells[rowIndex, 6].PutValue(dh.DaThanhToan);
                    ws.Cells[rowIndex, 6].SetStyle(orderNumStyle);
                    ws.Cells[rowIndex, 7].PutValue(dh.CongNo);
                    ws.Cells[rowIndex, 7].SetStyle(orderNumStyle);
                    ws.Cells[rowIndex, 8].PutValue(dh.IsDaThanhToan ? "Đã thanh toán" : "Chưa thanh toán");
                    ws.Cells[rowIndex, 8].SetStyle(orderStyle);
                    rowIndex++;

                    foreach (var sp in dh.SP_List)
                    {
                        ws.Cells.Merge(rowIndex, 0, 1, 2);
                        ws.Cells[rowIndex, 0].GetMergedRange().SetStyle(productStyle);
                        ws.Cells[rowIndex, 2].PutValue(sp.Ten_SP);
                        ws.Cells[rowIndex, 2].SetStyle(productStyle);
                        ws.Cells[rowIndex, 3].PutValue(sp.Dvt);
                        ws.Cells[rowIndex, 3].SetStyle(productStyle);
                        ws.Cells[rowIndex, 4].PutValue(sp.SoLuong);
                        ws.Cells[rowIndex, 4].SetStyle(productStyle);
                        ws.Cells[rowIndex, 5].PutValue(sp.ThanhTien);
                        ws.Cells[rowIndex, 5].SetStyle(productNumStyle);
                        ws.Cells.Merge(rowIndex, 6, 1, 3);
                        ws.Cells[rowIndex, 6].GetMergedRange().SetStyle(productStyle);
                        rowIndex++;
                    }
                }
            }

            Style totalStyle = new CellsFactory().CreateStyle();
            totalStyle.SetAllBorders();
            totalStyle.Font.IsBold = true;
            totalStyle.Pattern = BackgroundType.Solid;
            totalStyle.ForegroundColor = System.Drawing.Color.FromArgb(220, 230, 240);
            var totalNumStyle = new CellsFactory().CreateStyle();
            totalNumStyle.SetAllBorders();
            totalNumStyle.Font.IsBold = true;
            totalNumStyle.Pattern = BackgroundType.Solid;
            totalNumStyle.ForegroundColor = System.Drawing.Color.FromArgb(220, 230, 240);
            totalNumStyle.HorizontalAlignment = TextAlignmentType.Right;
            totalNumStyle.Custom = "#,##0";
            ws.Cells.Merge(rowIndex, 0, 1, 5);
            ws.Cells[rowIndex, 0].PutValue("TỔNG CỘNG");
            ws.Cells[rowIndex, 0].GetMergedRange().SetStyle(totalStyle);
            ws.Cells[rowIndex, 5].PutValue(data.Sum(x => x.DS_Ban));
            ws.Cells[rowIndex, 5].SetStyle(totalNumStyle);
            ws.Cells[rowIndex, 6].PutValue(data.Sum(x => x.DaThu));
            ws.Cells[rowIndex, 6].SetStyle(totalNumStyle);
            ws.Cells[rowIndex, 7].PutValue(data.Sum(x => x.CongNo));
            ws.Cells[rowIndex, 7].SetStyle(totalNumStyle);
            ws.Cells[rowIndex, 8].SetStyle(totalStyle);

            ws.AutoFitColumns();
            workbook.Save(stream, SaveFormat.Xlsx);
            return new OperationResult(true, stream.ToArray());
        }

        public async Task<TheoDoiNhanVienBanHang_Data> GetDataPagination(PaginationParams pagination, TheoDoiNhanVienBanHang_Param param)
        {
            var data = await GetData(param);
            var dataPagination = PaginationUtility<TheoDoiNhanVienBanHang_NV>.Create(data, pagination.PageNumber, pagination.PageSize);
            return new TheoDoiNhanVienBanHang_Data
            {
                FromDate_Str = Convert.ToDateTime(param.FromDate_Str).ToString("dd/MM/yyyy"),
                ToDate_Str = Convert.ToDateTime(param.ToDate_Str).ToString("dd/MM/yyyy"),
                Pagination = dataPagination.Pagination,
                Result = dataPagination.Result,
                Tong_So_Don = data.Sum(x => x.So_Don),
                Tong_SL_Ban = data.Sum(x => x.SL_Ban),
                Tong_DS_Ban = data.Sum(x => x.DS_Ban),
                Tong_DaThu = data.Sum(x => x.DaThu),
                Tong_CongNo = data.Sum(x => x.CongNo),
                SoDon_DaThanhToan = data.Sum(x => x.SoDon_DaThanhToan),
                SoDon_ChuaThanhToan = data.Sum(x => x.SoDon_ChuaThanhToan),
                SoDon_TreHan = data.Sum(x => x.SoDon_TreHan),
                NVs = string.Join(", ", data.Select(x => $"{x.Ten_NV} {x.SDT_NV}"))
            };
        }

        public async Task<List<TheoDoiNhanVienBanHang_NV>> GetData(TheoDoiNhanVienBanHang_Param param)
        {
            var fromDate = Convert.ToDateTime(param.FromDate_Str);
            var toDate = Convert.ToDateTime(param.ToDate_Str);

            var predicateDonHang = PredicateBuilder.New<DonHang>(x =>
                x.Loai == 2 && // Chỉ lấy đơn xuất hàng
                x.ID_NV.HasValue &&
                x.Date.HasValue && fromDate.Date <= x.Date.Value.Date && x.Date.Value.Date <= toDate.Date);

            if (param.IdNV != null && param.IdNV.Count > 0)
                predicateDonHang = predicateDonHang.And(x => x.ID_NV.HasValue && param.IdNV.Contains(x.ID_NV.Value));
            if (param.IdKH != null && param.IdKH.Count > 0)
                predicateDonHang = predicateDonHang.And(x => x.ID_KH.HasValue && param.IdKH.Contains(x.ID_KH.Value));
            if (!string.IsNullOrWhiteSpace(param.Ma_DH))
                predicateDonHang = predicateDonHang.And(x => x.Ma_DH != null && x.Ma_DH.Contains(param.Ma_DH.Trim()));

            var donHang = await _repoAccessor.DonHang.FindAll(predicateDonHang).AsNoTracking().ToListAsync();
            if (!donHang.Any()) return new List<TheoDoiNhanVienBanHang_NV>();

            var kh_ids = donHang.Where(x => x.ID_KH.HasValue).Select(x => x.ID_KH.Value).Distinct().ToList();
            var khachHang = await _repoAccessor.KhachHang.FindAll(x => kh_ids.Contains(x.ID)).AsNoTracking().ToListAsync();
            var khDict = khachHang.ToDictionary(x => x.ID);

            var today = DateTime.Now.Date;
            var donHangFiltered = donHang.Where(dh =>
            {
                var isPaid = (dh.TienMat ?? 0) + (dh.ChuyenKhoan ?? 0) >= (dh.TongTien ?? 0);
                if (param.TrangThai == "1") return isPaid;
                if (param.TrangThai == "2") return !isPaid;
                if (param.TrangThai == "4")
                {
                    if (isPaid) return false;
                    var soNgayCongNo = dh.SoNgayCongNo ??
                        (dh.ID_KH.HasValue && khDict.TryGetValue(dh.ID_KH.Value, out var kh) ? kh.SoNgayCongNo ?? 0 : 0);
                    return soNgayCongNo > 0 && dh.Date.HasValue && dh.Date.Value.Date.AddDays(soNgayCongNo).Date < today;
                }
                return true;
            }).ToList();

            if (!donHangFiltered.Any()) return new List<TheoDoiNhanVienBanHang_NV>();

            var donHang_ids = donHangFiltered.Select(x => x.ID).ToList();
            var chiTiet = await _repoAccessor.ChiTietDonHang.FindAll(x => donHang_ids.Contains(x.ID_DH)).AsNoTracking().ToListAsync();
            if (param.IdSP != null && param.IdSP.Count > 0)
                chiTiet = chiTiet.Where(x => param.IdSP.Contains(x.ID_SP)).ToList();

            var sp_ids = chiTiet.Select(x => x.ID_SP).Distinct().ToList();
            var sanPham = await _repoAccessor.SanPham.FindAll(x => sp_ids.Contains(x.ID)).AsNoTracking().ToListAsync();
            var spDict = sanPham.ToDictionary(x => x.ID);

            var nv_ids = donHangFiltered.Select(x => x.ID_NV).Distinct().ToList();
            var nhanVien = await _repoAccessor.NhanVien.FindAll(x => nv_ids.Contains(x.ID)).AsNoTracking().ToListAsync();
            var nvDict = nhanVien.ToDictionary(x => x.ID);

            var ctByDH = chiTiet.GroupBy(x => x.ID_DH).ToDictionary(g => g.Key, g => g.ToList());
            // Chỉ giữ các đơn có ít nhất 1 dòng chi tiết sau khi lọc sản phẩm
            var orders = donHangFiltered.Where(x => ctByDH.ContainsKey(x.ID)).ToList();
            var orderDTOs = orders.ToDictionary(o => o.ID, o => BuildOrder(o, khDict, today, ctByDH[o.ID], spDict));

            return orders
                .GroupBy(x => x.ID_NV)
                .Select(g => new TheoDoiNhanVienBanHang_NV
                {
                    ID_NV = g.Key ?? 0,
                    Ten_NV = g.Key.HasValue && nvDict.TryGetValue(g.Key.Value, out var nv) ? nv.Ten : "",
                    SDT_NV = g.Key.HasValue && nvDict.TryGetValue(g.Key.Value, out var nvSdt) ? nvSdt.SDT : "",
                    So_Don = g.Count(),
                    SL_Ban = g.Sum(o => ctByDH[o.ID].Sum(ct => ct.SoLuong)),
                    SoLoaiSP = g.SelectMany(o => ctByDH[o.ID].Select(ct => ct.ID_SP)).Distinct().Count(),
                    DS_Ban = g.Sum(o => ctByDH[o.ID].Sum(ct => ct.ThanhTien ?? 0)),
                    DaThu = g.Sum(o => (o.TienMat ?? 0) + (o.ChuyenKhoan ?? 0)),
                    CongNo = g.Sum(o => (o.TongTien ?? 0) - (o.TienMat ?? 0) - (o.ChuyenKhoan ?? 0)),
                    SoDon_DaThanhToan = g.Count(o => (o.TienMat ?? 0) + (o.ChuyenKhoan ?? 0) >= (o.TongTien ?? 0)),
                    SoDon_ChuaThanhToan = g.Count(o => (o.TienMat ?? 0) + (o.ChuyenKhoan ?? 0) < (o.TongTien ?? 0)),
                    SoDon_TreHan = g.Count(o => IsTreHan(o, khDict, today)),
                    DonHang_List = g.OrderByDescending(o => o.Date).Select(o => orderDTOs[o.ID]).ToList()
                })
                .OrderByDescending(x => x.DS_Ban)
                .ToList();
        }

        private TheoDoiNhanVienBanHang_DonHang BuildOrder(
            DonHang o,
            Dictionary<int, KhachHang> khDict,
            DateTime today,
            List<ChiTietDonHang> chiTiets,
            Dictionary<int, SanPham> spDict)
        {
            var tongTien = o.TongTien ?? 0;
            var daThanhToan = (o.TienMat ?? 0) + (o.ChuyenKhoan ?? 0);
            var isPaid = daThanhToan >= tongTien;
            var treHan = IsTreHan(o, khDict, today);
            return new TheoDoiNhanVienBanHang_DonHang
            {
                ID = o.ID,
                Ma_DH = o.Ma_DH,
                Date = o.Date,
                Ten_KH = o.ID_KH.HasValue && khDict.TryGetValue(o.ID_KH.Value, out var kh) ? kh.Ten : "",
                SoLoaiSP = chiTiets.Select(ct => ct.ID_SP).Distinct().Count(),
                TongTien = tongTien,
                DaThanhToan = daThanhToan,
                CongNo = tongTien - daThanhToan,
                IsDaThanhToan = isPaid,
                IsTreHan = !isPaid && treHan,
                SP_List = chiTiets.Select(ct => new TheoDoiNhanVienBanHang_SP
                {
                    Ten_SP = spDict.TryGetValue(ct.ID_SP, out var sp) ? sp.Ten : "",
                    Dvt = spDict.TryGetValue(ct.ID_SP, out var spDvt) ? spDvt.Dvt : "",
                    SoLuong = ct.SoLuong,
                    Gia = ct.Gia ?? 0,
                    ThanhTien = ct.ThanhTien ?? 0
                }).ToList()
            };
        }

        private bool IsTreHan(DonHang o, Dictionary<int, KhachHang> khDict, DateTime today)
        {
            var isPaid = (o.TienMat ?? 0) + (o.ChuyenKhoan ?? 0) >= (o.TongTien ?? 0);
            if (isPaid) return false;
            var soNgayCongNo = o.SoNgayCongNo ??
                (o.ID_KH.HasValue && khDict.TryGetValue(o.ID_KH.Value, out var kh) ? kh.SoNgayCongNo ?? 0 : 0);
            return soNgayCongNo > 0 && o.Date.HasValue && o.Date.Value.Date.AddDays(soNgayCongNo).Date < today;
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

        public async Task<List<KeyValuePair<int, string>>> GetListKhachHang()
        {
            var khachHang = await _repoAccessor.KhachHang.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.Ma_KH})"))
                .Distinct()
                .ToListAsync();
            return khachHang;
        }
    }
}
