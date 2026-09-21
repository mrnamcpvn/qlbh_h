using API.Models;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_NhaCungCap
    {
        Task<PaginationUtility<NhaCungCap>> GetDataPagination(PaginationParam pagination, string name);
        Task<OperationResult> Create(NhaCungCap model);
        Task<OperationResult> Update(NhaCungCap model);
        Task<bool> Delete(int id);
        Task<List<NhaCungCap>> GetAll();
        Task<OperationResult> Template();
        Task<OperationResult> Upload(IFormFile file);
    }
}