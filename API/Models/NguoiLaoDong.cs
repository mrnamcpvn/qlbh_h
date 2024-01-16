
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public partial class NguoiLaoDong
    {
        [Key]
        public int ID { get; set; }
        [StringLength(150)]
        public string Name { get; set; }
       
    }
}
