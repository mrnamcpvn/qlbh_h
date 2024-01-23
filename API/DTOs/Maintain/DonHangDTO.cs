using API.Models;

namespace API.DTOs.Maintain
{
    public class DonHangDTO : DonHang
    {
        public List<ChiTietDonHang> ChiTiet { get;set; }
    }
}