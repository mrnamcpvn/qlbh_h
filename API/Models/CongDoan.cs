using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public partial class CongDoan
    {
        [Key]
        public int ID { get; set; }
        
        [StringLength(150)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(13, 3)")]
        public decimal Money {get; set;}
        public int? IDMaHang { get; set; }
       
    }
}