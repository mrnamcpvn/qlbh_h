using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_SanPham
    {
       Task<PaginationUtility<SanPham>> GetDataPagination(PaginationParams pagination, string name);
        Task<bool> Create(SanPham model);
        Task<bool> Delete(int id);
        Task<bool> Update(SanPham model);
        Task<List<SanPham>> GetAll();
       //Task<List<KeyValuePair<int, string>>> GetAllByCommodityCodeId(int id);
    }
}