using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_User
    {
        Task<PaginationUtility<NguoiDung>> GetDataPagination(PaginationParams pagination, string name);
        Task<bool> Create(NguoiDung model);
        Task<bool> Delete(int id);
        Task<bool> Update(NguoiDung model);
    }
}