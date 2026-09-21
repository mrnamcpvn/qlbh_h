using API.Models;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_User
    {
        Task<PaginationUtility<NguoiDung>> GetDataPagination(PaginationParam pagination, string name);
        Task<bool> Create(NguoiDung model);
        Task<bool> Delete(int id);
        Task<bool> Update(NguoiDung model);
    }
}