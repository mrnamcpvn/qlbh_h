using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
     public partial class ChamCong
    {
        [Key]
        public int ID { get; set; }
        public int ID_NLD { get; set; }
        public int ID_CD { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
       
    }
}