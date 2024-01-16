using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_CalculateWages
    {
        Task<PaginationUtility<ChamCongDTO>> GetDataPagination(PaginationParams pagination, string fromDate, string toDate);
        Task<bool> Create(ChamCong model);
        Task<bool> Delete(int id);
        Task<bool> Update(ChamCong model);
        Task<ChamCongDTO> GetDetail(int id);
    }
}