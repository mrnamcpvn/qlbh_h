using static SD3_API.Helpers.Utilities.PaginationUtility<API.DTOs.Maintain.TheoDoiNhanVienBanHang_SP>;

namespace API.DTOs.Maintain
{
    public class TheoDoiNhanVienBanHang_Param
    {
        public string FromDate_Str { get; set; }
        public string ToDate_Str { get; set; }
        public List<int> IdNV { get; set; }
        public List<int> IdSP { get; set; }
    }
    public class TheoDoiNhanVienBanHang_Data
    {
        public string FromDate_Str { get; set; }
        public string ToDate_Str { get; set; }
        public int Tong_SL_Ban { get; set; }
        public decimal Tong_DS_Ban { get; set; }
        public PaginationResult Pagination { get; set; }
        public List<TheoDoiNhanVienBanHang_SP> Result { get; set; }
        public string NVs { get; set; }
    }
    public class TheoDoiNhanVienBanHang_SP
    {
        public string Ten_SP { get; set; }
        public int SL_Ban { get; set; }
        public decimal DS_Ban { get; set; }
        public List<TheoDoiNhanVienBanHang_NV> NV_List { get; set; }
    }
    public class TheoDoiNhanVienBanHang_NV
    {
        public string Ten_NV { get; set; }
        public string SDT_NV { get; set; }
        public string DVT { get; set; }
        public int SL_Ban { get; set; }
        public decimal DS_Ban { get; set; }
    }
}