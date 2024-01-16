namespace API.DTOs.Maintain
{
    public class CongDoanDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Money {get; set;}
        public int? IDMaHang { get; set; }
        public string MaHang { get; set; }
    }
}