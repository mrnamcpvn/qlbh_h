using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_NhaCungCap
    {
        Task<PaginationUtility<NhaCungCap>> GetDataPagination(PaginationParams pagination, string name);
        Task<OperationResult> Create(NhaCungCap model);
        Task<OperationResult> Update(NhaCungCap model);
        Task<bool> Delete(int id);
        Task<List<NhaCungCap>> GetAll();
        Task<OperationResult> Template();
        Task<OperationResult> Upload(IFormFile file);
    }
}