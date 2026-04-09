using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helper.Utilities;
using API.Helpers.Params;
using API.Models;
using Aspose.Cells;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_KhachHang : I_KhachHang
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_KhachHang(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<KhachHang>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<KhachHang>(true);


            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Ten.Contains(name));
            }
            var data = _repoAccessor.KhachHang.FindAll(predicateUser);
            var result = await PaginationUtility<KhachHang>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<OperationResult> Create(KhachHang model)
        {
            var isExist = await _repoAccessor.KhachHang.FindAll(x => x.Ma_KH == model.Ma_KH).AnyAsync();
            if (isExist)
                return new OperationResult(false, "Mã khách hàng này đã tồn tại trong hệ thống.");

            _repoAccessor.KhachHang.Add(model);
            await _repoAccessor.Save();
            return new OperationResult(true);
        }

        public async Task<OperationResult> Update(KhachHang model)
        {
            var isDuplicate = await _repoAccessor.KhachHang.FindAll(x => x.Ma_KH == model.Ma_KH && x.ID != model.ID).AnyAsync();
            if (isDuplicate)
                return new OperationResult(false, "Mã khách hàng đã được sử dụng bởi một khách hàng khác.");

            _repoAccessor.KhachHang.Update(model);
            await _repoAccessor.Save();
            return new OperationResult(true);
        }

        public async Task<bool> Delete(int id)
        {
            var kh = await _repoAccessor.KhachHang.FindSingle(x => x.ID == id);
            if (kh != null)
            {
                _repoAccessor.KhachHang.Remove(kh);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<List<KhachHang>> GetAll()
        {
            var data = await _repoAccessor.KhachHang.FindAll().ToListAsync();
            return data;
        }
        public Task<OperationResult> Template()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Resources\\Template\\KhachHang\\Template.xlsx");
            Workbook workbook = new(path);
            WorkbookDesigner design = new(workbook);
            MemoryStream stream = new();
            design.Workbook.Save(stream, SaveFormat.Xlsx);
            return Task.FromResult(new OperationResult(true, stream.ToArray())); ;
        }

        public async Task<OperationResult> Upload(IFormFile file)
        {
            ExcelResult resp = ExcelUtility.CheckExcel(file, $"Resources\\Template\\KhachHang\\Template.xlsx");
            if (!resp.IsSuccess)
                return new OperationResult(false, resp.Error);
            (bool isPassed, List<KhachHang> excelDataList, List<KhachHangDTO> excelReportList)
                = CheckUploadData(resp);
            if (!isPassed)
            {
                MemoryStream memoryStream = new();
                string fileLocation = Path.Combine(AppContext.BaseDirectory, $"Resources\\Template\\KhachHang\\Report.xlsx");
                WorkbookDesigner workbookDesigner = new() { Workbook = new Workbook(fileLocation) };
                Worksheet worksheet = workbookDesigner.Workbook.Worksheets[0];
                workbookDesigner.SetDataSource("result", excelReportList);
                workbookDesigner.Process();
                worksheet.AutoFitColumns(worksheet.Cells.MinDataColumn, worksheet.Cells.MaxColumn);
                worksheet.AutoFitRows(worksheet.Cells.MinDataRow + 1, worksheet.Cells.MaxRow);
                workbookDesigner.Workbook.Save(memoryStream, SaveFormat.Xlsx);
                return new OperationResult { IsSuccess = false, Data = memoryStream.ToArray(), Error = "Có lỗi xảy ra trong quá trình xử lí dữ liệu, vui lòng kiểm tra file báo cáo" };
            }
            try
            {
                if (excelDataList.Any())
                {
                    var _maKH = excelDataList.Select(x => x.Ma_KH).ToList();
                    var existingKHs = await _repoAccessor.KhachHang.FindAll(x => _maKH.Contains(x.Ma_KH)).ToListAsync();
                    var toAdd = new List<KhachHang>();
                    var distinctExcelData = excelDataList.GroupBy(x => x.Ma_KH).Select(g => g.Last()).ToList();
                    foreach (var item in distinctExcelData)
                    {
                        var dbItem = existingKHs.FirstOrDefault(x => x.Ma_KH == item.Ma_KH);
                        if (dbItem != null)
                        {
                            dbItem.Ten = item.Ten;
                            dbItem.SDT = item.SDT;
                            dbItem.DiaChi = item.DiaChi;
                            dbItem.Email = item.Email;
                            _repoAccessor.KhachHang.Update(dbItem);
                        }
                        else
                            toAdd.Add(item);
                    }
                    if (toAdd.Any())
                        _repoAccessor.KhachHang.AddMultiple(toAdd);
                    await _repoAccessor.Save();
                    string path = "uploaded\\KhachHang";
                    await FilesUtility.SaveFile(file, path, $"KhachHang_{DateTime.Now:yyyyMMddHHmmss}");
                }
                return new OperationResult(true);
            }
            catch (Exception)
            {
                return new OperationResult(false, "Đã xảy ra lỗi khi thêm dữ liệu mới");
            }
        }
        public (bool isPassed, List<KhachHang> excelDataList, List<KhachHangDTO> excelReportList) CheckUploadData(ExcelResult resp)
        {
            List<KhachHang> excelDataList = new();
            List<KhachHangDTO> excelReportList = new();
            bool isPassed = true;
            for (int i = resp.WsTemp.Cells.Rows.Count; i < resp.Ws.Cells.Rows.Count; i++)
            {
                KhachHangDTO report = new()
                {
                    Ma_KH = resp.Ws.Cells[i, 0].StringValue.Trim(),
                    Ten = resp.Ws.Cells[i, 1].StringValue.Trim(),
                    SDT = resp.Ws.Cells[i, 2].StringValue.Trim(),
                    DiaChi = resp.Ws.Cells[i, 3].StringValue.Trim(),
                    Email = resp.Ws.Cells[i, 4].StringValue.Trim(),
                    Error = ""
                };
                excelReportList.Add(report);
                if (string.IsNullOrWhiteSpace(report.Ma_KH))
                    report.Error += $"Cột [Mã Khách Hàng] không có giá trị.\n";
                if (!string.IsNullOrWhiteSpace(report.Ma_KH) && report.Ten.Length > 250)
                    report.Error += $"Cột [Mã Khách Hàng] vượt số lượng kí tự cho phép.\n";

                if (string.IsNullOrWhiteSpace(report.Ten))
                    report.Error += $"Cột [Tên Khách Hàng] không có giá trị.\n";
                if (!string.IsNullOrWhiteSpace(report.Ten) && report.Ten.Length > 250)
                    report.Error += $"Cột [Tên Khách Hàng] vượt số lượng kí tự cho phép.\n";

                if (!string.IsNullOrWhiteSpace(report.SDT) && report.SDT.Length > 20)
                    report.Error += $"Cột [Số Điện Thoại] vượt số lượng kí tự cho phép.\n";
                if (!string.IsNullOrWhiteSpace(report.DiaChi) && report.DiaChi.Length > 500)
                    report.Error += $"Cột [Địa Chỉ] vượt số lượng kí tự cho phép.\n";
                if (!string.IsNullOrWhiteSpace(report.Email) && report.Email.Length > 100)
                    report.Error += $"Cột [Email] vượt số lượng kí tự cho phép.\n";
                if (string.IsNullOrWhiteSpace(report.Error))
                {
                    KhachHang excelData = new()
                    {
                        Ma_KH = report.Ma_KH,
                        Ten = report.Ten,
                        SDT = report.SDT,
                        DiaChi = report.DiaChi,
                        Email = report.Email,
                    };
                    excelDataList.Add(excelData);
                }
                else
                {
                    isPassed = false;
                    report.Error = report.Error[..^1];
                }
            }
            return (isPassed, excelDataList, excelReportList);
        }
    }
}