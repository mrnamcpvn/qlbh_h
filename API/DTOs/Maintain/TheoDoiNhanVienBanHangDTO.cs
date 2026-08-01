using static SD3_API.Helpers.Utilities.PaginationUtility<API.DTOs.Maintain.TheoDoiNhanVienBanHang_NV>;

namespace API.DTOs.Maintain
{
    public class TheoDoiNhanVienBanHang_Param
    {
        public string FromDate_Str { get; set; }
        public string ToDate_Str { get; set; }
        public List<int> IdNV { get; set; }
        public List<int> IdSP { get; set; }
        public List<int> IdKH { get; set; }
        public string Ma_DH { get; set; }
        // "1" = Đã thanh toán, "2" = Chưa thanh toán, "4" = Trễ hạn
        public string TrangThai { get; set; }
    }
    public class TheoDoiNhanVienBanHang_Data
    {
        public string FromDate_Str { get; set; }
        public string ToDate_Str { get; set; }
        public int Tong_So_Don { get; set; }
        public int Tong_SL_Ban { get; set; }
        public decimal Tong_DS_Ban { get; set; }
        public decimal Tong_DaThu { get; set; }
        public decimal Tong_CongNo { get; set; }
        public int SoDon_DaThanhToan { get; set; }
        public int SoDon_ChuaThanhToan { get; set; }
        public int SoDon_TreHan { get; set; }
        public PaginationResult Pagination { get; set; }
        public List<TheoDoiNhanVienBanHang_NV> Result { get; set; }
        public string NVs { get; set; }
    }
    public class TheoDoiNhanVienBanHang_NV
    {
        public int ID_NV { get; set; }
        public string Ten_NV { get; set; }
        public string SDT_NV { get; set; }
        public int So_Don { get; set; }
        public int SL_Ban { get; set; }
        public int SoLoaiSP { get; set; }
        public decimal DS_Ban { get; set; }
        public decimal DaThu { get; set; }
        public decimal CongNo { get; set; }
        public int SoDon_DaThanhToan { get; set; }
        public int SoDon_ChuaThanhToan { get; set; }
        public int SoDon_TreHan { get; set; }
        public List<TheoDoiNhanVienBanHang_DonHang> DonHang_List { get; set; }
    }
    public class TheoDoiNhanVienBanHang_DonHang
    {
        public int ID { get; set; }
        public string Ma_DH { get; set; }
        public DateTime? Date { get; set; }
        public string Ten_KH { get; set; }
        public int SoLoaiSP { get; set; }
        public decimal TongTien { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal CongNo { get; set; }
        public bool IsDaThanhToan { get; set; }
        public bool IsTreHan { get; set; }
        public List<TheoDoiNhanVienBanHang_SP> SP_List { get; set; }
    }
    public class TheoDoiNhanVienBanHang_SP
    {
        public string Ten_SP { get; set; }
        public string Dvt { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public decimal ThanhTien { get; set; }
    }
}
