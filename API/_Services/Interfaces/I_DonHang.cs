using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_DonHang
    {
        Task<PaginationUtility<DonHang>> GetDataPagination(PaginationParams pagination,string fromDate, string toDate, int type);
        Task<bool> Create(DonHang model);
        Task<bool> Delete(int id);
        Task<bool> Update(DonHang model);
        Task<List<ChiTietDonHang>> GetDetail(int id);
    }
}