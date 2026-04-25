using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;
using System.Collections.Generic;

namespace API.DTOs.Maintain
{
    public class InfoDTO
    {
        public int ID { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
    }
    public class DonHangO : DonHang
    {
        public string Ten_KH { get; set; }
        public string Ten_NCC { get; set; }
        public string Ten_NV { get; set; }
        public string Date_Str { get; set; }
        public string DiaChi { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; }
    }
    public class DonHangDTO : DonHangO
    {
        public List<ChiTietDonHang> ChiTiet { get; set; }
    }

    public class DonHangRequestDTO
    {
        public PaginationParams Pagination { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int Loai { get; set; }
        public string TinhTrang { get; set; }
        public string Ma_DH { get; set; }
        public int? PayType { get; set; }
        public int? DateType { get; set; }
    }

    public class DonHangPaginationResult
    {
        public PaginationUtility<DonHangO> Pagination { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class ChiTietDonHangDTO : ChiTietDonHang
    {
        public string Ten_SP { get; set; }
        public string Ma_DH { get; set; }
        public string Dvt { get; set; }
    }

}