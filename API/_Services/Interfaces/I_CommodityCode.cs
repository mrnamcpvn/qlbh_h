using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_CommodityCode
    {
        Task<PaginationUtility<MaHang>> GetDataPagination(PaginationParams pagination, string name);
        Task<bool> Create(string name);
        Task<bool> Delete(int id);
        Task<bool> Update(MaHang model);
        Task<List<MaHang>> GetAll();
    }
}