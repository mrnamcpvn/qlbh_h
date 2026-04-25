using static SD3_API.Helpers.Utilities.PaginationUtility<API.DTOs.Maintain.LichSuHangHoaItem>;

namespace API.DTOs.Maintain
{
    public class LichSuHangHoaParam
    {
        public List<int> IdSP { get; set; }
        public List<int> IdKH { get; set; }
        public List<int> IdNCC { get; set; }
        public List<int> IdNV { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? Loai { get; set; }
    }

    public class LichSuHangHoaItem
    {
        public int ID { get; set; }
        public int ID_SP { get; set; }
        public string MaSP { get; set; }
        public string Ten_SP { get; set; }
        public string Dvt { get; set; }
        public int ID_DH { get; set; }
        public string Ma_DH { get; set; }
        public int Loai { get; set; }
        public string LoaiStr { get; set; }
        public string DoiTac { get; set; }
        public string Ten_NV { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public decimal ThanhTien { get; set; }
        public int? SL_Ton_Dau { get; set; }
        public int? SL_Ton_Cuoi { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Updated_Time { get; set; }
    }

    public class LichSuHangHoaData
    {
        public PaginationResult Pagination { get; set; }
        public List<LichSuHangHoaItem> Result { get; set; }
        public int TongSLNhap { get; set; }
        public int TongSLXuat { get; set; }
        public decimal TongTienNhap { get; set; }
        public decimal TongTienXuat { get; set; }
    }
}
