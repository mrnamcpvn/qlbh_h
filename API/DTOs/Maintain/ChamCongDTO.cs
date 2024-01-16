using API.Models;

namespace API.DTOs.Maintain
{
    public class ChamCongDTO : ChamCong
    {
        public string NLD { get; set; }
        public string CD_Name { get; set; }
        public int IDMaHang { get; set; }
        public string MH_Name { get; set; }
    }
}