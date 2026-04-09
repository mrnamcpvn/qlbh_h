using API.Helpers.Params;
using API.Models;
using System.Collections.Generic;

namespace API.DTOs.Maintain
{
    public class DonHangO: DonHang
    {

        public string DiaChi { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; }
    }
    public class DonHangDTO : DonHangO
    {
        public List<ChiTietDonHang> ChiTiet { get;set; }
    }

    public class DonHangRequestDTO
    {
        public PaginationParams Pagination { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Loai { get; set; }
        public string TinhTrang { get; set; }
        public int? SoHoaDon { get; set; }
        public int? PayType { get; set; }
    }
}