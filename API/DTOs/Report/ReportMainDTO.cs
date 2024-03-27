namespace API.DTOs.Report
{
    public class Report
    {
        public int ID_SP { get; set; }
        public string Ten_SP { get; set; }
        public int SoLuong { get; set; }
        public int? SLTonDau { get; set; }
        public int? SLTonCuoi { get; set; }
        public decimal? Gia { get; set; }
        public int? Loai { get; set; }
        public DateTime? Updated_time { get; set; }
    }

    public class ReportDTO
    {
        public int ID_SP { get; set; }
        public string Ten_SP { get; set; }
        public int SoLuongNhap { get; set; }
        public int SoLuongXuat { get; set; }
        public int? SoLuongTonDau { get; set; }
        public int? SoLuongTonCuoi { get; set; }
        public decimal? TongTienNhap { get; set; }
        public decimal? GiaTon { get; set; }
        public decimal? TongTienXuat { get; set; }
        public decimal? DoanhThu { get; set; }
    }

    public class ReportParam
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int  ID_SP { get; set; }
    }
}