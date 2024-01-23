using AgileObjects.AgileMapper.Configuration;
using API.DTOs.Maintain;
using API.Models;

namespace API.Helper.Mappers
{
    public class MapperConfig : MapperConfiguration
    {
        protected override void Configure()
        {
             WhenMapping.From<DonHang>().To<DonHangDTO>();
        }
    }
}