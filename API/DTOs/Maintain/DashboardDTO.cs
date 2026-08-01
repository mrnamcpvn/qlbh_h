namespace API.DTOs.Maintain
{
    public class OverdueDonHangDTO
    {
        public int ID { get; set; }
        public string Ma_DH { get; set; }
        public string Ten_KH { get; set; }
        public string SDT_KH { get; set; }
        public DateTime? Date { get; set; }
        public string DateStr { get; set; }
        public DateTime? NgayDenHan { get; set; }
        public string NgayDenHanStr { get; set; }
        public decimal TongTien { get; set; }
        public decimal DaTT { get; set; }
        public decimal ConNo { get; set; }
        public int SoNgayTre { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class CongNoCustomerSummaryDTO
    {
        public int KhachHangID { get; set; }
        public string TenKhachHang { get; set; }
        public string SDT { get; set; }
        public int SoDonNo { get; set; }
        public decimal TongConNo { get; set; }
        public decimal? HanMucCongNo { get; set; }
        public string TrangThai { get; set; }
        public string TrangThaiText { get; set; }
    }

    public class DashboardSummaryDTO
    {
        public string FilterType { get; set; } = "month";
        public string FilterPeriodName { get; set; } = "";
        public string PrevPeriodName { get; set; } = "";

        // Kỳ chọn (Ngày / Tuần / Tháng / Quý / Năm)
        public decimal TongThuKy { get; set; }
        public decimal TongChiKy { get; set; }
        public decimal TongThuKyTruoc { get; set; }
        public decimal TongChiKyTruoc { get; set; }

        // Tương thích ngược
        public decimal TongThuThang { get => TongThuKy; set => TongThuKy = value; }
        public decimal TongChiThang { get => TongChiKy; set => TongChiKy = value; }
        public decimal TongThuThangTruoc { get => TongThuKyTruoc; set => TongThuKyTruoc = value; }
        public decimal TongChiThangTruoc { get => TongChiKyTruoc; set => TongChiKyTruoc = value; }

        public decimal TongDuNo { get; set; }
        public int SoDonQuaHan { get; set; }
        public int SoDonChuaThanhToan { get; set; }

        // Danh sách đơn quá hạn
        public List<OverdueDonHangDTO> DonQuaHan { get; set; } = new();

        // Top công nợ khách hàng
        public List<CongNoCustomerSummaryDTO> TopCongNoKhachHang { get; set; } = new();

        // Thống kê đồ thị theo các mốc thời gian của kỳ
        public List<DashboardDayStatDTO> RevenueByDay { get; set; } = new();
    }

    public class DashboardDayStatDTO
    {
        public string Label { get; set; }
        public decimal TongThu { get; set; }
        public decimal TongChi { get; set; }
    }
}
