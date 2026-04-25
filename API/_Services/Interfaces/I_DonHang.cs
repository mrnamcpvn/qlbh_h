using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_DonHang
    {
        Task<DonHangPaginationResult> GetDataPagination(DonHangRequestDTO filter);
        Task<OperationResult> DownloadExcel(DonHangRequestDTO filter);
        Task<DonHangO> Create(DonHangDTO model);
        Task<bool> Delete(int id);
        Task<bool> DeleteItem(int id);
        Task<DonHangO> Update(DonHangDTO model);
        Task<bool> UpdatePayment(DonHang model);
        Task<List<ChiTietDonHangDTO>> GetDetail(int id);
    }
}