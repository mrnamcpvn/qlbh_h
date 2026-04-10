
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public partial class CuaHang
    {
        [Key]
        public int ID { get; set; }
        [StringLength(250)]
        public string Ten { get; set; }
        [StringLength(20)]
        public string SDT { get; set; }
        [StringLength(500)]
        public string DiaChi { get; set; }
       
    }
}
