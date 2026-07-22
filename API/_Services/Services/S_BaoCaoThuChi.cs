using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helpers.Utilities;
using API.Models;
using Aspose.Cells;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_BaoCaoThuChi : I_BaoCaoThuChi
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_BaoCaoThuChi(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<BaoCaoThuChiData> GetData(BaoCaoThuChiParam param)
        {
            var orderItems = await GetOrders(param);
            var fromDate = Convert.ToDateTime(param.FromDate);
            var toDate = Convert.ToDateTime(param.ToDate);

            return new BaoCaoThuChiData
            {
                FromDateStr = fromDate.ToString("dd/MM/yyyy"),
                ToDateStr = toDate.ToString("dd/MM/yyyy"),
                Orders = orderItems,
                TongTongTien = orderItems.Sum(x => x.Loai == 1 ? -x.TongTien : x.TongTien),
                TongTienMat = orderItems.Sum(x => x.TienMat),
                TongChuyenKhoan = orderItems.Sum(x => x.ChuyenKhoan),
                TongConThieu = orderItems.Sum(x => x.ConThieu)
            };
        }

        public async Task<OperationResult> Excel(BaoCaoThuChiParam param)
        {
            var orderItems = await GetOrders(param);
            if (!orderItems.Any()) return new OperationResult(false, "Không có dữ liệu");

            var fromDate = Convert.ToDateTime(param.FromDate);
            var toDate = Convert.ToDateTime(param.ToDate);

            MemoryStream stream = new();
            Workbook workbook = new();
            Worksheet ws = workbook.Worksheets[0];
            ws.Name = "BaoCaoThuChi";

            Style style = new CellsFactory().CreateStyle();
            style.Font.Name = "Calibri";
            style.Font.Size = 16;
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Center;
            style.VerticalAlignment = TextAlignmentType.Center;

            ws.Cells.Merge(0, 0, 1, 10);
            ws.Cells[0, 0].PutValue("BÁO CÁO THU CHI");
            ws.Cells[0, 0].GetMergedRange().SetStyle(style);

            style.Font.Size = 11;
            style.HorizontalAlignment = TextAlignmentType.Center;
            ws.Cells.Merge(1, 0, 1, 10);
            ws.Cells[1, 0].PutValue($"Từ ngày {fromDate:dd/MM/yyyy} đến ngày {toDate:dd/MM/yyyy}");
            ws.Cells[1, 0].GetMergedRange().SetStyle(style);

            var headers = new[] { "Ngày", "Đơn Hàng", "Đối tác", "NV", "Sản phẩm", "Đvt", "SL", "Thành tiền", "Thanh toán", "Còn lại" };
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
            foreach (var order in orderItems)
            {
                var isImport = order.Loai == 1;
                var headerRow = rowIndex;
                var productCount = order.Products.Count;

                style.Font.IsBold = true;
                style.Custom = null;
                style.HorizontalAlignment = TextAlignmentType.Left;
                ws.Cells[headerRow, 0].PutValue(order.DateStr);
                ws.Cells[headerRow, 0].SetStyle(style);
                ws.Cells[headerRow, 1].PutValue($"{(isImport ? "Nhập" : "Xuất")}{(string.IsNullOrEmpty(order.Ma_DH) ? "" : $" - {order.Ma_DH}")}");
                ws.Cells[headerRow, 1].SetStyle(style);
                ws.Cells[headerRow, 2].PutValue(order.DoiTac);
                ws.Cells[headerRow, 2].SetStyle(style);
                ws.Cells[headerRow, 3].PutValue(order.NhanVien);
                ws.Cells[headerRow, 3].SetStyle(style);

                ws.Cells.Merge(headerRow, 4, 1, 3);
                ws.Cells[headerRow, 4].PutValue($"{productCount} sản phẩm");
                ws.Cells[headerRow, 4].GetMergedRange().SetStyle(style);

                style.Custom = "#,##0";
                style.HorizontalAlignment = TextAlignmentType.Right;
                ws.Cells[headerRow, 7].PutValue($"{(isImport ? "-" : "+")}{order.TongTien:#,##0}");
                ws.Cells[headerRow, 7].SetStyle(style);

                var thanhToan = "";
                if (order.TienMat > 0) thanhToan += $"TM: {order.TienMat:#,##0}";
                if (order.ChuyenKhoan > 0) thanhToan += (thanhToan.Length > 0 ? " / " : "") + $"CK: {order.ChuyenKhoan:#,##0}";
                ws.Cells[headerRow, 8].PutValue(thanhToan);
                style.HorizontalAlignment = TextAlignmentType.Left;
                ws.Cells[headerRow, 8].SetStyle(style);
                style.HorizontalAlignment = TextAlignmentType.Right;
                if (order.ConThieu < 0)
                    ws.Cells[headerRow, 9].PutValue($"Đã Thanh Toán (Thừa {-order.ConThieu:#,##0})");
                else if (order.ConThieu == 0)
                    ws.Cells[headerRow, 9].PutValue("Đã Thanh Toán");
                else
                    ws.Cells[headerRow, 9].PutValue(order.ConThieu);
                ws.Cells[headerRow, 9].SetStyle(style);

                rowIndex++;

                style.Font.IsBold = false;
                style.Custom = null;
                style.HorizontalAlignment = TextAlignmentType.Left;

                if (productCount > 1)
                {
                    ws.Cells.Merge(rowIndex, 0, productCount, 4);
                    ws.Cells.Merge(rowIndex, 8, productCount, 2);
                }

                foreach (var sp in order.Products)
                {
                    style.Custom = null;
                    style.HorizontalAlignment = TextAlignmentType.Left;
                    ws.Cells[rowIndex, 4].PutValue(sp.TenSP);
                    ws.Cells[rowIndex, 4].SetStyle(style);
                    ws.Cells[rowIndex, 5].PutValue(sp.Dvt);
                    style.HorizontalAlignment = TextAlignmentType.Center;
                    ws.Cells[rowIndex, 5].SetStyle(style);
                    ws.Cells[rowIndex, 6].PutValue(sp.SoLuong);
                    ws.Cells[rowIndex, 6].SetStyle(style);
                    style.Custom = "#,##0";
                    style.HorizontalAlignment = TextAlignmentType.Right;
                    ws.Cells[rowIndex, 7].PutValue(sp.ThanhTien);
                    ws.Cells[rowIndex, 7].SetStyle(style);

                    if (productCount <= 1)
                    {
                        style.Custom = null;
                        style.HorizontalAlignment = TextAlignmentType.Left;
                        ws.Cells[rowIndex, 0].PutValue("");
                        ws.Cells[rowIndex, 0].SetStyle(style);
                        ws.Cells[rowIndex, 1].PutValue("");
                        ws.Cells[rowIndex, 1].SetStyle(style);
                        ws.Cells[rowIndex, 2].PutValue("");
                        ws.Cells[rowIndex, 2].SetStyle(style);
                        ws.Cells[rowIndex, 3].PutValue("");
                        ws.Cells[rowIndex, 3].SetStyle(style);
                        ws.Cells[rowIndex, 8].PutValue("");
                        ws.Cells[rowIndex, 8].SetStyle(style);
                        ws.Cells[rowIndex, 9].PutValue("");
                        ws.Cells[rowIndex, 9].SetStyle(style);
                    }

                    rowIndex++;
                }
            }

            var tongTongTien = orderItems.Sum(x => x.Loai == 1 ? -x.TongTien : x.TongTien);
            style.Font.IsBold = true;
            style.HorizontalAlignment = TextAlignmentType.Right;
            style.Custom = "#,##0";
            ws.Cells.Merge(rowIndex, 0, 1, 4);
            ws.Cells[rowIndex, 0].PutValue("Tổng Cộng");
            style.HorizontalAlignment = TextAlignmentType.Center;
            ws.Cells[rowIndex, 0].GetMergedRange().SetStyle(style);
            style.HorizontalAlignment = TextAlignmentType.Right;
            ws.Cells[rowIndex, 7].PutValue(tongTongTien);
            ws.Cells[rowIndex, 7].SetStyle(style);
            ws.Cells[rowIndex, 8].PutValue("");
            ws.Cells[rowIndex, 8].SetStyle(style);
            ws.Cells[rowIndex, 9].PutValue("");
            ws.Cells[rowIndex, 9].SetStyle(style);

            var dataRange = ws.Cells.CreateRange(2, 0, rowIndex - 1, 10);
            var borderStyle = new CellsFactory().CreateStyle();
            borderStyle.SetAllBorders();
            dataRange.ApplyStyle(borderStyle, new StyleFlag { Borders = true });

            ws.AutoFitColumns();
            ws.Cells.SetColumnWidth(8, 30);
            workbook.Save(stream, SaveFormat.Xlsx);
            return new OperationResult(true, stream.ToArray());
        }

        private async Task<List<BaoCaoThuChiOrder>> GetOrders(BaoCaoThuChiParam param)
        {
            var fromDate = Convert.ToDateTime(param.FromDate);
            var toDate = Convert.ToDateTime(param.ToDate);

            var donHangPredicate = PredicateBuilder.New<DonHang>(x =>
                (x.Loai == 1 || x.Loai == 2) &&
                x.Date.HasValue &&
                fromDate.Date <= x.Date.Value.Date &&
                x.Date.Value.Date <= toDate.Date
            );

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

            if (param.Loai == 1)
                donHangPredicate = donHangPredicate.And(x => x.Loai == 1);
            else if (param.Loai == 2)
                donHangPredicate = donHangPredicate.And(x => x.Loai == 2);

            var orders = await _repoAccessor.DonHang.FindAll(donHangPredicate)
                .AsNoTracking().ToListAsync();

            if (!orders.Any()) return new List<BaoCaoThuChiOrder>();

            var orderIds = orders.Select(x => x.ID).ToList();

            var usedOrderIds = orderIds;
            if (param.IdSP != null && param.IdSP.Count > 0)
            {
                var spFilteredChiTiets = await _repoAccessor.ChiTietDonHang
                    .FindAll(x => orderIds.Contains(x.ID_DH) && param.IdSP.Contains(x.ID_SP))
                    .AsNoTracking().ToListAsync();
                usedOrderIds = spFilteredChiTiets.Select(x => x.ID_DH).Distinct().ToList();
            }
            orders = orders.Where(x => usedOrderIds.Contains(x.ID)).ToList();

            if (!orders.Any()) return new List<BaoCaoThuChiOrder>();

            var allChiTiets = await _repoAccessor.ChiTietDonHang
                .FindAll(x => usedOrderIds.Contains(x.ID_DH))
                .AsNoTracking().ToListAsync();

            var customerIds = orders.Where(x => x.ID_KH.HasValue).Select(x => x.ID_KH.Value).Distinct().ToList();
            var customers = customerIds.Any()
                ? await _repoAccessor.KhachHang.FindAll(x => customerIds.Contains(x.ID))
                    .AsNoTracking().ToDictionaryAsync(x => x.ID, x => x.Ten)
                : new Dictionary<int, string>();

            var nccIds = orders.Where(x => x.ID_NCC.HasValue).Select(x => x.ID_NCC.Value).Distinct().ToList();
            var nccs = nccIds.Any()
                ? await _repoAccessor.NhaCungCap.FindAll(x => nccIds.Contains(x.ID))
                    .AsNoTracking().ToDictionaryAsync(x => x.ID, x => x.Ten)
                : new Dictionary<int, string>();

            var nvIds = orders.Where(x => x.ID_NV.HasValue).Select(x => x.ID_NV.Value).Distinct().ToList();
            var nhanViens = nvIds.Any()
                ? await _repoAccessor.NhanVien.FindAll(x => nvIds.Contains(x.ID))
                    .AsNoTracking().ToDictionaryAsync(x => x.ID, x => x.Ten)
                : new Dictionary<int, string>();

            var orderDict = orders.ToDictionary(x => x.ID);

            var spIds = allChiTiets.Select(x => x.ID_SP).Distinct().ToList();
            var spDict = spIds.Any()
                ? await _repoAccessor.SanPham.FindAll(x => spIds.Contains(x.ID))
                    .AsNoTracking().ToDictionaryAsync(x => x.ID)
                : new Dictionary<int, SanPham>();

            var orderItems = allChiTiets
                .GroupBy(x => x.ID_DH)
                .Select(g =>
                {
                    var order = orderDict[g.Key];
                    var tongTien = order.TongTien ?? 0;
                    var tienMat = order.TienMat ?? 0;
                    var chuyenKhoan = order.ChuyenKhoan ?? 0;

                    var doiTac = order.Loai == 1
                        ? (order.ID_NCC.HasValue && nccs.ContainsKey(order.ID_NCC.Value)
                            ? nccs[order.ID_NCC.Value] : "")
                        : (order.ID_KH.HasValue && customers.ContainsKey(order.ID_KH.Value)
                            ? customers[order.ID_KH.Value] : "");

                    var nhanVien = order.ID_NV.HasValue && nhanViens.ContainsKey(order.ID_NV.Value)
                        ? nhanViens[order.ID_NV.Value] : "";

                    var products = g.Select(ct =>
                    {
                        var sp = spDict.GetValueOrDefault(ct.ID_SP);
                        return new BaoCaoThuChiProductDetail
                        {
                            TenSP = sp != null ? $"{sp.Ten} ({sp.MaSP})" : "",
                            Dvt = sp?.Dvt ?? "",
                            SoLuong = ct.SoLuong,
                            ThanhTien = ct.ThanhTien ?? 0,
                        };
                    }).ToList();

                    return new BaoCaoThuChiOrder
                    {
                        Date = order.Date,
                        DateStr = order.Date.HasValue ? order.Date.Value.ToString("dd/MM/yyyy") : "",
                        Ma_DH = order.Ma_DH,
                        Loai = order.Loai ?? 0,
                        DoiTac = doiTac,
                        NhanVien = nhanVien,
                        Products = products,
                        TongTien = tongTien,
                        TienMat = tienMat,
                        ChuyenKhoan = chuyenKhoan,
                        ConThieu = tongTien - tienMat - chuyenKhoan,
                    };
                })
                .OrderBy(x => x.Date)
                .ThenBy(x => x.Ma_DH)
                .ToList();

            return orderItems;
        }

        public async Task<List<KeyValuePair<int, string>>> GetListSanPham()
        {
            return await _repoAccessor.SanPham.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.MaSP})"))
                .Distinct().ToListAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListKhachHang()
        {
            return await _repoAccessor.KhachHang.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.Ma_KH})"))
                .Distinct().ToListAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListNhaCungCap()
        {
            return await _repoAccessor.NhaCungCap.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, $"{x.Ten} ({x.Ma_NCC})"))
                .Distinct().ToListAsync();
        }

        public async Task<List<KeyValuePair<int, string>>> GetListNhanVien()
        {
            return await _repoAccessor.NhanVien.FindAll().AsNoTracking().OrderBy(x => x.ID)
                .Select(x => new KeyValuePair<int, string>(x.ID, x.Ten))
                .Distinct().ToListAsync();
        }
    }
}
