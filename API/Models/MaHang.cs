using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public partial class MaHang
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
    }
}