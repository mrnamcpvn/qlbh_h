using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_Employee
    {
        Task<PaginationUtility<NguoiLaoDong>> GetDataPagination(PaginationParams pagination, string name);
        Task<bool> Create(string name);
        Task<bool> Update(NguoiLaoDong model);
        Task<bool> Delete(int id);
        Task<List<NguoiLaoDong>> GetAll();
    }
}