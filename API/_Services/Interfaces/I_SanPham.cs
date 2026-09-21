using API.DTOs.Maintain;
using API.Models;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_SanPham
    {
       Task<PaginationUtility<SanPham>> GetDataPagination(PaginationParam pagination, string name);
        Task<OperationResult> Create(SanPham model);
        Task<bool> Delete(int id);
        Task<OperationResult> Update(SanPham model);
        Task<List<SanPham>> GetAll();
        Task<OperationResult> Template();
        Task<OperationResult> Upload(IFormFile file);
       //Task<List<KeyValuePair<int, string>>> GetAllByCommodityCodeId(int id);
    }
}