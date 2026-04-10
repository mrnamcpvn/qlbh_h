using Aspose.Cells;

namespace API.Helper.Utilities
{

    public static class ExcelUtility
    {
        private static readonly string rootPath = Directory.GetCurrentDirectory();

        public static ExcelResult CheckExcel(IFormFile file, string subPath, int initRow = 0, int initCol = 0)
        {
            if (file == null)
                return new ExcelResult(false, "Không tìm thấy file");
            using Stream stream = file.OpenReadStream();
            WorkbookDesigner designer = new()
            {
                Workbook = new Workbook(stream)
            };
            Worksheet ws = designer.Workbook.Worksheets[0];
            if (designer.Workbook.Worksheets.Count > 1)
                return new ExcelResult(false, "File excel có nhiều hơn một sheet");
            string pathTemp = Path.Combine(rootPath, subPath);
            designer.Workbook = new Workbook(pathTemp);
            Worksheet wsTemp = designer.Workbook.Worksheets[0];
            ws.Cells.DeleteBlankColumns();
            ws.Cells.DeleteBlankRows();
            wsTemp.Cells.DeleteBlankColumns();
            wsTemp.Cells.DeleteBlankRows();
            if (ws.Cells.MaxDataColumn < wsTemp.Cells.MaxDataColumn)
                return new ExcelResult(false, "Thiếu cột dữ liệu");
            if (ws.Cells.MaxDataColumn > wsTemp.Cells.MaxDataColumn)
                return new ExcelResult(false, "Không đủ cột dữ liệu cần thiết");
            if (ws.Cells.MaxDataRow <= wsTemp.Cells.MaxDataRow)
                return new ExcelResult(false, "File excel không có dữ liệu");
            string firstCellTemp = wsTemp.Cells[0, 0].Name;
            string lastCellTemp = wsTemp.Cells[wsTemp.Cells.MaxDataRow, wsTemp.Cells.MaxDataColumn].Name;
            Aspose.Cells.Range rangeTemp = wsTemp.Cells.CreateRange(firstCellTemp, lastCellTemp);
            Aspose.Cells.Range range = ws.Cells.CreateRange(firstCellTemp, lastCellTemp);
            for (int r = initRow; r < rangeTemp.RowCount; r++)
            {
                for (int c = initCol; c < rangeTemp.ColumnCount; c++)
                {
                    string val = range[r, c].Value != null ? range[r, c].StringValue.Trim() : "";
                    string valTmp = rangeTemp[r, c].Value != null ? rangeTemp[r, c].StringValue.Trim() : "";
                    if (val != valTmp)
                        return new ExcelResult(false, $"Tiêu đề tại ô {CellsHelper.CellIndexToName(r, c)} : {val}\nPhải bằng : {valTmp}");
                }
            }
            return new ExcelResult(true, ws, wsTemp);
        }

        public static ExcelResult DownloadExcel<T>(List<T> data, string subPath, ConfigDownload configDownload = null)
        {
            configDownload ??= new ConfigDownload();
            if (!data.Any())
                return new ExcelResult(false, "Không có dữ liệu");
            try
            {
                MemoryStream stream = new();
                var path = Path.Combine(rootPath, subPath);
                WorkbookDesigner designer = new() { Workbook = new Workbook(path) };
                Worksheet ws = designer.Workbook.Worksheets[0];
                designer.SetDataSource("result", data);
                designer.Process();
                if (configDownload.IsAutoFitColumn) ws.AutoFitColumns(ws.Cells.MinDataColumn, ws.Cells.MaxColumn);
                designer.Workbook.Save(stream, configDownload.SaveFormat);
                return new ExcelResult(true, stream.ToArray());
            }
            catch (Exception ex)
            {
                return new ExcelResult(false, ex.InnerException.Message);
            }
        }
        public static ExcelResult DownloadExcel(List<Table> dataTable, List<Cell> dataCell, string subPath, ConfigDownload configDownload = null)
        {
            configDownload ??= new ConfigDownload();
            if (!dataTable.Any() && !dataCell.Any())
                return new ExcelResult(false, "Không có dữ liệu");
            try
            {
                MemoryStream stream = new();
                string path = Path.Combine(rootPath, subPath);
                WorkbookDesigner designer = new() { Workbook = new Workbook(path) };
                Worksheet ws = designer.Workbook.Worksheets[0];
                if (dataTable.Any())
                    dataTable.ForEach(item => designer.SetDataSource(item.Root, item.Data));
                designer.Process();
                if (dataCell.Any())
                    dataCell.ForEach(item =>
                    {
                        ws.Cells[item.Location].PutValue(item.Value);
                        if (item.IsStyle) ws.Cells[item.Location].SetStyle(item.Style);
                    });
                if (configDownload.IsAutoFitColumn) ws.AutoFitColumns(ws.Cells.MinDataColumn, ws.Cells.MaxColumn);
                designer.Workbook.Save(stream, configDownload.SaveFormat);
                return new ExcelResult(true, stream.ToArray());
            }
            catch (Exception ex)
            {
                return new ExcelResult(false, ex.InnerException.Message);
            }
        }

        public static ExcelResult DownloadExcel(List<Table> dataTable, string subPath, ConfigDownload configDownload = null)
        {
            configDownload ??= new ConfigDownload();
            if (!dataTable.Any())
                return new ExcelResult(false, "Không có dữ liệu");
            try
            {
                MemoryStream stream = new();
                string path = Path.Combine(rootPath, subPath);
                WorkbookDesigner designer = new() { Workbook = new Workbook(path) };
                Worksheet ws = designer.Workbook.Worksheets[0];
                dataTable.ForEach(item => designer.SetDataSource(item.Root, item.Data));
                designer.Process();
                if (configDownload.IsAutoFitColumn) ws.AutoFitColumns(ws.Cells.MinDataColumn, ws.Cells.MaxColumn);
                designer.Workbook.Save(stream, configDownload.SaveFormat);
                return new ExcelResult(true, stream.ToArray());
            }
            catch (Exception ex)
            {
                return new ExcelResult(false, ex.InnerException.Message);
            }
        }

        public static ExcelResult DownloadExcel(List<Cell> dataCell, string subPath, ConfigDownload configDownload = null)
        {
            configDownload ??= new ConfigDownload();
            if (!dataCell.Any())
                return new ExcelResult(false, "Không có dữ liệu");
            try
            {
                MemoryStream stream = new();
                string path = Path.Combine(rootPath, subPath);
                WorkbookDesigner designer = new() { Workbook = new Workbook(path) };
                Worksheet ws = designer.Workbook.Worksheets[0];
                dataCell.ForEach(item =>
                {
                    ws.Cells[item.Location].PutValue(item.Value);
                    if (item.IsStyle) ws.Cells[item.Location].SetStyle(item.Style);
                });
                if (configDownload.IsAutoFitColumn) ws.AutoFitColumns(ws.Cells.MinDataColumn, ws.Cells.MaxColumn);
                designer.Workbook.Save(stream, configDownload.SaveFormat);
                return new ExcelResult(true, stream.ToArray());
            }
            catch (Exception ex)
            {
                return new ExcelResult(false, ex.InnerException.Message);
            }
        }
    }
    public class ExcelResult
    {
        public string Error { set; get; }
        public bool IsSuccess { set; get; }
        public Worksheet Ws { set; get; }
        public Worksheet WsTemp { set; get; }
        public byte[] Result { get; set; }

        public ExcelResult(bool isSuccess, Worksheet worksheet, Worksheet worksheetTemp)
        {
            IsSuccess = isSuccess;
            Ws = worksheet;
            WsTemp = worksheetTemp;
        }
        public ExcelResult(bool isSuccess, string error)
        {
            Error = error;
            IsSuccess = isSuccess;
        }
        public ExcelResult(bool isSuccess, byte[] result)
        {
            IsSuccess = isSuccess;
            Result = result;
        }
        public ExcelResult()
        {
        }
    }
    public class Cell
    {
        public string Location { get; set; }
        public object Value { get; set; }
        public Style Style { get; set; }
        public bool IsStyle { get; private set; }

        public Cell(string location, object value)
        {
            Location = location;
            Value = value;
            IsStyle = false;
        }
        public Cell(int row, int column, object value)
        {
            Location = CellsHelper.CellIndexToName(row, column);
            Value = value;
            IsStyle = false;
        }

        public Cell(string location, object value, Style style)
        {
            Location = location;
            Value = value;
            Style = style;
            IsStyle = true;
        }
        public Cell(int row, int column, object value, Style style)
        {
            Location = CellsHelper.CellIndexToName(row, column);
            Value = value;
            Style = style;
            IsStyle = true;
        }
    }
    public class Table
    {
        public string Root { get; set; }
        public object Data { get; set; }

        public Table(string root, object data)
        {
            Root = root;
            Data = data;
        }
    }
    public class ConfigDownload
    {
        public bool IsAutoFitColumn { get; set; } = true;
        public SaveFormat SaveFormat { get; set; } = SaveFormat.Xlsx;
        public ConfigDownload() { }
        public ConfigDownload(bool isAutoFitColumn)
        {
            this.IsAutoFitColumn = isAutoFitColumn;
        }
        public ConfigDownload(SaveFormat saveFormat)
        {
            this.SaveFormat = saveFormat;
        }
        public ConfigDownload(bool isAutoFitColumn, SaveFormat saveFormat)
        {
            this.IsAutoFitColumn = isAutoFitColumn;
            this.SaveFormat = saveFormat;
        }
    }
}