using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_SanPham
    {
       Task<PaginationUtility<SanPham>> GetDataPagination(PaginationParams pagination, string name);
        Task<OperationResult> Create(SanPham model);
        Task<bool> Delete(int id);
        Task<OperationResult> Update(SanPham model);
        Task<List<SanPham>> GetAll();
        Task<OperationResult> Template();
        Task<OperationResult> Upload(IFormFile file);
       //Task<List<KeyValuePair<int, string>>> GetAllByCommodityCodeId(int id);
    }
}