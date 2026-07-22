namespace API.DTOs.Maintain
{
    public class CongNoSummaryDTO
    {
        public int KhachHangID { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public int SoNgayCongNo { get; set; }
        public int SoDonNo { get; set; }
        public decimal TongConNo { get; set; }
        public DateTime? NgayDenHanGanNhat { get; set; }
        public string TrangThai { get; set; }
    }

    public class CongNoChiTietDTO
    {
        public int DonHangID { get; set; }
        public string Ma_DH { get; set; }
        public DateTime NgayXuat { get; set; }
        public DateTime NgayDenHan { get; set; }
        public decimal TongTien { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal ConNo { get; set; }
        public int SoNgayQuaHan { get; set; }
    }

    public class CongNoFilterDTO
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<int> IdKH { get; set; }
        public string TrangThai { get; set; }
    }

    public class CongNoCustomerDTO
    {
        public int KhachHangID { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public int SoNgayCongNo { get; set; }
        public decimal? HanMucCongNo { get; set; }
        public int SoDonNo { get; set; }
        public decimal TongConNo { get; set; }
        public DateTime? NgayDenHanGanNhat { get; set; }
        public string TrangThai { get; set; }
        public List<CongNoChiTietDTO> Orders { get; set; }
    }

    public class CustomerDebtInfoDTO
    {
        public decimal CurrentDebt { get; set; }
        public decimal? CreditLimit { get; set; }
        public int SoNgayCongNo { get; set; }
        public bool HasOverdueOrders { get; set; }
        public int OverdueOrderCount { get; set; }
        public bool IsOverLimit { get; set; }
        public decimal RemainingCredit { get; set; }
    }
}
