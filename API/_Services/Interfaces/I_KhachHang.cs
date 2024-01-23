using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_KhachHang
    {
        Task<PaginationUtility<KhachHang>> GetDataPagination(PaginationParams pagination, string name);
        Task<bool> Create(KhachHang model);
        Task<bool> Update(KhachHang model);
        Task<bool> Delete(int id);
        Task<List<KhachHang>> GetAll();
    }
}