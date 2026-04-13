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
    public class S_NhaCungCap : I_NhaCungCap
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_NhaCungCap(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<NhaCungCap>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<NhaCungCap>(true);


            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Ten.Contains(name));
            }
            var data = _repoAccessor.NhaCungCap.FindAll(predicateUser);
            var result = await PaginationUtility<NhaCungCap>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<OperationResult> Create(NhaCungCap model)
        {
            var isExist = await _repoAccessor.NhaCungCap.FindAll(x => x.Ma_NCC == model.Ma_NCC).AnyAsync();
            if (isExist)
                return new OperationResult(false, "Mã nhà cung cấp này đã tồn tại trong hệ thống.");

            _repoAccessor.NhaCungCap.Add(model);
            await _repoAccessor.Save();
            return new OperationResult(true);
        }

        public async Task<OperationResult> Update(NhaCungCap model)
        {
            var isDuplicate = await _repoAccessor.NhaCungCap.FindAll(x => x.Ma_NCC == model.Ma_NCC && x.ID != model.ID).AnyAsync();
            if (isDuplicate)
                return new OperationResult(false, "Mã nhà cung cấp đã được sử dụng bởi một nhà cung cấp NCCác.");

            _repoAccessor.NhaCungCap.Update(model);
            await _repoAccessor.Save();
            return new OperationResult(true);
        }

        public async Task<bool> Delete(int id)
        {
            var NCC = await _repoAccessor.NhaCungCap.FindSingle(x => x.ID == id);
            if (NCC != null)
            {
                _repoAccessor.NhaCungCap.Remove(NCC);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<List<NhaCungCap>> GetAll()
        {
            var data = await _repoAccessor.NhaCungCap.FindAll().ToListAsync();
            return data;
        }
        public Task<OperationResult> Template()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Resources\\Template\\NhaCungCap\\Template.xlsx");
            Workbook workbook = new(path);
            WorkbookDesigner design = new(workbook);
            MemoryStream stream = new();
            design.Workbook.Save(stream, SaveFormat.Xlsx);
            return Task.FromResult(new OperationResult(true, stream.ToArray())); ;
        }

        public async Task<OperationResult> Upload(IFormFile file)
        {
            ExcelResult resp = ExcelUtility.CheckExcel(file, $"Resources\\Template\\NhaCungCap\\Template.xlsx");
            if (!resp.IsSuccess)
                return new OperationResult(false, resp.Error);
            (bool isPassed, List<NhaCungCap> excelDataList, List<NhaCungCapDTO> excelReportList)
                = CheckUploadData(resp);
            if (!isPassed)
            {
                MemoryStream memoryStream = new();
                string fileLocation = Path.Combine(AppContext.BaseDirectory, $"Resources\\Template\\NhaCungCap\\Report.xlsx");
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
                    var _maNCC = excelDataList.Select(x => x.Ma_NCC).ToList();
                    var existingNCCs = await _repoAccessor.NhaCungCap.FindAll(x => _maNCC.Contains(x.Ma_NCC)).ToListAsync();
                    var toAdd = new List<NhaCungCap>();
                    var distinctExcelData = excelDataList.GroupBy(x => x.Ma_NCC).Select(g => g.Last()).ToList();
                    foreach (var item in distinctExcelData)
                    {
                        var dbItem = existingNCCs.FirstOrDefault(x => x.Ma_NCC == item.Ma_NCC);
                        if (dbItem != null)
                        {
                            dbItem.Ten = item.Ten;
                            dbItem.SDT = item.SDT;
                            dbItem.DiaChi = item.DiaChi;
                            dbItem.Email = item.Email;
                            _repoAccessor.NhaCungCap.Update(dbItem);
                        }
                        else
                            toAdd.Add(item);
                    }
                    if (toAdd.Any())
                        _repoAccessor.NhaCungCap.AddMultiple(toAdd);
                    await _repoAccessor.Save();
                    string path = "uploaded\\NhaCungCap";
                    await FilesUtility.SaveFile(file, path, $"NhaCungCap_{DateTime.Now:yyyyMMddHHmmss}");
                }
                return new OperationResult(true);
            }
            catch (Exception)
            {
                return new OperationResult(false, "Đã xảy ra lỗi khi thêm dữ liệu mới");
            }
        }
        public (bool isPassed, List<NhaCungCap> excelDataList, List<NhaCungCapDTO> excelReportList) CheckUploadData(ExcelResult resp)
        {
            List<NhaCungCap> excelDataList = new();
            List<NhaCungCapDTO> excelReportList = new();
            bool isPassed = true;
            for (int i = resp.WsTemp.Cells.Rows.Count; i < resp.Ws.Cells.Rows.Count; i++)
            {
                NhaCungCapDTO report = new()
                {
                    Ma_NCC = resp.Ws.Cells[i, 0].StringValue.Trim(),
                    Ten = resp.Ws.Cells[i, 1].StringValue.Trim(),
                    DiaChi= resp.Ws.Cells[i, 2].StringValue.Trim(),
                    SDT = resp.Ws.Cells[i, 3].StringValue.Trim(),
                    Email = resp.Ws.Cells[i, 4].StringValue.Trim(),
                    Error = ""
                };
                excelReportList.Add(report);
                if (string.IsNullOrWhiteSpace(report.Ma_NCC))
                    report.Error += $"Cột [Mã Nhà Cung Cấp] không có giá trị.\n";
                if (!string.IsNullOrWhiteSpace(report.Ma_NCC) && report.Ma_NCC.Length > 20)
                    report.Error += $"Cột [Mã Nhà Cung Cấp] vượt số lượng kí tự cho phép.\n";

                if (string.IsNullOrWhiteSpace(report.Ten))
                    report.Error += $"Cột [Tên Nhà Cung Cấp] không có giá trị.\n";
                if (!string.IsNullOrWhiteSpace(report.Ten) && report.Ten.Length > 250)
                    report.Error += $"Cột [Tên Nhà Cung Cấp] vượt số lượng kí tự cho phép.\n";

                if (!string.IsNullOrWhiteSpace(report.SDT) && report.SDT.Length > 20)
                    report.Error += $"Cột [Số Điện Thoại] vượt số lượng kí tự cho phép.\n";
                if (!string.IsNullOrWhiteSpace(report.DiaChi) && report.DiaChi.Length > 500)
                    report.Error += $"Cột [Địa Chỉ] vượt số lượng kí tự cho phép.\n";
                if (!string.IsNullOrWhiteSpace(report.Email) && report.Email.Length > 100)
                    report.Error += $"Cột [Email] vượt số lượng kí tự cho phép.\n";
                if (string.IsNullOrWhiteSpace(report.Error))
                {
                    NhaCungCap excelData = new()
                    {
                        Ma_NCC = report.Ma_NCC,
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