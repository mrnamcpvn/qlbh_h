namespace API.DTOs.Maintain
{
    public class BaoCaoThuChiParam
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<int> IdSP { get; set; }
        public List<int> IdKH { get; set; }
        public List<int> IdNCC { get; set; }
        public List<int> IdNV { get; set; }
        public int Loai { get; set; }
    }

    public class BaoCaoThuChiProductDetail
    {
        public string TenSP { get; set; }
        public string Dvt { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class BaoCaoThuChiOrder
    {
        public DateTime? Date { get; set; }
        public string DateStr { get; set; }
        public string Ma_DH { get; set; }
        public int Loai { get; set; }
        public string DoiTac { get; set; }
        public string NhanVien { get; set; }
        public List<BaoCaoThuChiProductDetail> Products { get; set; }
        public decimal TongTien { get; set; }
        public decimal TienMat { get; set; }
        public decimal ChuyenKhoan { get; set; }
        public decimal ConThieu { get; set; }
    }

    public class BaoCaoThuChiData
    {
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public List<BaoCaoThuChiOrder> Orders { get; set; }
        public decimal TongTongTien { get; set; }
        public decimal TongTienMat { get; set; }
        public decimal TongChuyenKhoan { get; set; }
        public decimal TongConThieu { get; set; }
    }
}
