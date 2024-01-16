using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_StepInProcess
    {
        Task<PaginationUtility<CongDoanDTO>> GetDataPagination(PaginationParams pagination, string name);
        Task<bool> Create(CongDoan model);
        Task<bool> Delete(int id);
        Task<bool> Update(CongDoan model);
        Task<List<KeyValuePair<int, string>>> GetAll();
        Task<List<KeyValuePair<int, string>>> GetAllByCommodityCodeId(int id);
    }
}