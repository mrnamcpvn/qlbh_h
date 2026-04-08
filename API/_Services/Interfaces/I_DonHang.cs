using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_DonHang
    {
        Task<PaginationUtility<DonHangO>> GetDataPagination(DonHangRequestDTO filter);
        Task<OperationResult> DownloadExcel(DonHangRequestDTO filter);
        Task<DonHang> Create(DonHangDTO model);
        Task<bool> Delete(int id);
        Task<bool> DeleteItem(int id);
        Task<DonHang> Update(DonHangDTO model);
        Task<bool> ChangeStatus(DonHang model);
        Task<bool> UpdatePayment(DonHang model);
        Task<List<ChiTietDonHang>> GetDetail(int id);
    }
}