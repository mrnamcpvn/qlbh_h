using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_KhachHang
    {
        Task<PaginationUtility<KhachHang>> GetDataPagination(PaginationParams pagination, string name);
        Task<OperationResult> Create(KhachHang model);
        Task<OperationResult> Update(KhachHang model);
        Task<bool> Delete(int id);
        Task<List<KhachHang>> GetAll();
        Task<OperationResult> Template();
        Task<OperationResult> Upload(IFormFile file);
    }
}