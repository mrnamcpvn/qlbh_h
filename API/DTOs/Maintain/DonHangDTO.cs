using API.Models;

namespace API.DTOs.Maintain
{
    public class DonHangO: DonHang
    {
        public string DiaChi { get;set; }
    }
    public class DonHangDTO : DonHangO
    {
        public List<ChiTietDonHang> ChiTiet { get;set; }
    }
}