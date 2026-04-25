using API.DTOs.Maintain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API._Services.Interfaces
{
    public interface I_CongNo
    {
        Task<List<CongNoSummaryDTO>> GetSummary();
        Task<List<CongNoChiTietDTO>> GetDetail(int khachHangID);
        Task<List<CongNoCustomerDTO>> GetData(CongNoFilterDTO filter);
        Task<CustomerDebtInfoDTO> GetCustomerDebtInfo(int khachHangID);
    }
}
