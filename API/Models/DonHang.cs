using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
     public partial class DonHang
    {
        [Key]
        public int ID { get; set; }
        public int? Loai { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? TongTien { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? TienMat { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? ChuyenKhoan { get; set; }
        public int? ID_NV { get; set; }
        public string Ma_DH { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Create_Time { get; set; }
        public int? ID_KH { get; set; }
        public int? ID_NCC { get; set; }
    }
}