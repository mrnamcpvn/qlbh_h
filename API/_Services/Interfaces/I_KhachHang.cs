using API.Models;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_KhachHang
    {
        Task<PaginationUtility<KhachHang>> GetDataPagination(PaginationParam pagination, string name);
        Task<OperationResult> Create(KhachHang model);
        Task<OperationResult> Update(KhachHang model);
        Task<bool> Delete(int id);
        Task<List<KhachHang>> GetAll();
        Task<OperationResult> Template();
        Task<OperationResult> Upload(IFormFile file);
    }
}