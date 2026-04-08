using System.Transactions;
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
    public class S_SanPham : I_SanPham
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_SanPham(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<SanPham>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<SanPham>(true);

            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Ten.Contains(name) || x.MaSP.Contains(name));
            }
            var data = _repoAccessor.SanPham.FindAll(predicateUser).OrderBy(x => x.MaSP);
            var result = await PaginationUtility<SanPham>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        // public async Task<PaginationUtility<SanPhamDTO>> GetDataPagination(PaginationParams pagination, string name)
        // {
        //     var predicateUser = PredicateBuilder.New<SanPham>(true);

        //     if (!string.IsNullOrEmpty(name))
        //     {
        //         predicateUser.And(x => x.Name.Trim().Contains(name));
        //     }
        //     var data = _repoAccessor.SanPham.FindAll(predicateUser)
        //         .Join(_repoAccessor.MaHang.FindAll(),
        //             x => x.IDMaHang,
        //             y => y.ID,
        //             (x, y) => new { SanPham = x, maHang = y }
        //         ).Select(x => new SanPhamDTO
        //         {
        //             ID = x.SanPham.ID,
        //             IDMaHang = x.SanPham.IDMaHang,
        //             MaHang = x.maHang.Name,
        //             Money = x.SanPham.Money,
        //             Name = x.SanPham.Name
        //         }).OrderBy(x => x.MaHang);
        //     var result = await PaginationUtility<SanPhamDTO>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
        //     return result;
        // }

        public async Task<bool> Create(SanPham model)
        {
            _repoAccessor.SanPham.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var cd = await _repoAccessor.SanPham.FindSingle(x => x.ID == id);
            if (cd != null)
            {
                _repoAccessor.SanPham.Remove(cd);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Update(SanPham model)
        {
            _repoAccessor.SanPham.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<List<SanPham>> GetAll()
        {
            var data = await _repoAccessor.SanPham.FindAll().ToListAsync();
            return data;
        }

        public Task<OperationResult> Template()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "Resources\\Template\\SanPham\\Template.xlsx");
            Workbook workbook = new(path);
            WorkbookDesigner design = new(workbook);
            MemoryStream stream = new();
            design.Workbook.Save(stream, SaveFormat.Xlsx);
            return Task.FromResult(new OperationResult(true, stream.ToArray())); ;
        }

        public async Task<OperationResult> Upload(IFormFile file)
        {
            ExcelResult resp = ExcelUtility.CheckExcel(file, $"Resources\\Template\\SanPham\\Template.xlsx");
            if (!resp.IsSuccess)
                return new OperationResult(false, resp.Error);
            (bool isPassed, List<SanPham> excelDataList, List<SanPhamDTO> excelReportList)
                = CheckUploadData(resp);
            if (!isPassed)
            {
                MemoryStream memoryStream = new();
                string fileLocation = Path.Combine(AppContext.BaseDirectory, $"Resources\\Template\\SanPham\\Report.xlsx");
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
                    _repoAccessor.SanPham.AddMultiple(excelDataList);
                    await _repoAccessor.Save();
                    string path = "uploaded\\SanPham";
                    await FilesUtility.SaveFile(file, path, $"SanPham_{DateTime.Now:yyyyMMddHHmmss}");
                }
                return new OperationResult(true);
            }
            catch (Exception)
            {
                return new OperationResult(false, "Đã xảy ra lỗi khi thêm dữ liệu mới");
            }
        }
        public (bool isPassed, List<SanPham> excelDataList, List<SanPhamDTO> excelReportList) CheckUploadData(ExcelResult resp)
        {
            List<SanPham> excelDataList = new();
            List<SanPhamDTO> excelReportList = new();
            bool isPassed = true;
            for (int i = resp.WsTemp.Cells.Rows.Count; i < resp.Ws.Cells.Rows.Count; i++)
            {
                SanPhamDTO report = new()
                {
                    MaSP = resp.Ws.Cells[i, 0].StringValue.Trim(),
                    Ten = resp.Ws.Cells[i, 1].StringValue.Trim(),
                    Dvt = resp.Ws.Cells[i, 2].StringValue.Trim(),
                    Gia = resp.Ws.Cells[i, 3].StringValue.Trim(),
                    SoLuong = resp.Ws.Cells[i, 4].StringValue.Trim(),
                    Error = ""
                };
                excelReportList.Add(report);
                //MaSP
                if (!string.IsNullOrWhiteSpace(report.MaSP) && report.MaSP.Length > 100)
                    report.Error += $"Cột [Mã Sản Phẩm] vượt số lượng kí tự cho phép.\n";
                //Ten
                if (!string.IsNullOrWhiteSpace(report.Ten) && report.Ten.Length > 250)
                    report.Error += $"Cột [Tên Sản Phẩm] vượt số lượng kí tự cho phép.\n";
                // Gia
                if (!string.IsNullOrWhiteSpace(report.Gia) && !decimal.TryParse(report.Gia, out _))
                    report.Error += $"Giá trị cột [Giá] phải là số nguyên.\n";
                //Dvt
                if (!string.IsNullOrWhiteSpace(report.Dvt) && report.Dvt.Length > 50)
                    report.Error += $"Cột [Đơn Vị Tính] vượt số lượng kí tự cho phép.\n";
                // SoLuong
                if (!string.IsNullOrWhiteSpace(report.SoLuong) && !int.TryParse(report.SoLuong, out _))
                    report.Error += $"Giá trị cột [Số Lượng] phải là số nguyên.\n";
                if (string.IsNullOrWhiteSpace(report.Error))
                {
                    SanPham excelData = new()
                    {
                        MaSP = report.MaSP,
                        Ten = report.Ten,
                        Gia = decimal.Parse(report.Gia),
                        Dvt = report.Dvt,
                        SoLuong = int.Parse(report.SoLuong),
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
        // public async Task<List<KeyValuePair<int, string>>> GetAllByCommodityCodeId(int id)
        // {
        //     var data = await _repoAccessor.SanPham.FindAll(x => x.IDMaHang == id).Select(x => new KeyValuePair<int, string>(x.ID, x.Name)).ToListAsync();
        //     return data;
        // }
    }
}