using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public partial class NguoiDung
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        public string TaiKhoan { get; set; }
        [Required]
        [StringLength(150)]
        public string MatKhau { get; set; }
        [StringLength(250)]
        public string HoTen { get; set; }
    }
}