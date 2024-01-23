using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
     public partial class ChiTietDonHang
    {
        [Key]
        public int ID { get; set; }
        public int ID_DH { get; set; }
        public int ID_SP { get; set; }
        public string Ten_SP { get; set; }
        public int SoLuong { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Gia { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? ThanhTien { get; set; }
       
    }
}