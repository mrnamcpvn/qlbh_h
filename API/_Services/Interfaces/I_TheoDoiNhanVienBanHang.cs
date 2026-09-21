using API.DTOs.Maintain;
using API.Models;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_TheoDoiNhanVienBanHang
    {
        Task<TheoDoiNhanVienBanHang_Data> GetDataPagination(PaginationParam pagination, TheoDoiNhanVienBanHang_Param param);
        Task<OperationResult> Excel(TheoDoiNhanVienBanHang_Param param);
        Task<List<KeyValuePair<int, string>>> GetListSanPham();
        Task<List<KeyValuePair<int, string>>> GetListNhanVien();
        Task<List<KeyValuePair<int, string>>> GetListKhachHang();
    }
}
