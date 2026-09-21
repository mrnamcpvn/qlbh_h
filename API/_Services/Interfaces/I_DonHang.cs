using API.DTOs.Maintain;
using API.Models;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_DonHang
    {
        Task<DonHangPaginationResult> GetDataPagination(DonHangRequestDTO filter);
        Task<OperationResult> DownloadExcel(DonHangRequestDTO filter);
        Task<DonHangO> Create(DonHangDTO model);
        Task<bool> Delete(int id);
        Task<bool> DeleteItem(int id);
        Task<DonHangO> Update(DonHangDTO model);
        Task<bool> UpdatePayment(DonHang model);
        Task<DonHangO> GetById(int id);
        Task<List<ChiTietDonHangDTO>> GetDetail(int id);
        Task<List<KeyValuePair<int, string>>> GetListNhaCungCap();
        Task<List<KeyValuePair<int, string>>> GetListKhachHang();
    }
}