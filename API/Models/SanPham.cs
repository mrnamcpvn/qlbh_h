using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public partial class SanPham
    {
        [Key]
        public int ID { get; set; }
        [StringLength(100)]
        public string MaSP { get; set; }
        [StringLength(250)]
        public string Ten { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Gia { get; set; }
        [StringLength(50)]
        public string Dvt { get; set; }
        public int? SoLuong {get; set;}
       
    }
}