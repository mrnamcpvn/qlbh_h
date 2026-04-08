using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_TheoDoiNhanVienBanHang
    {
        Task<TheoDoiNhanVienBanHang_Data> GetDataPagination(PaginationParams pagination, TheoDoiNhanVienBanHang_Param param);
       Task<OperationResult> Excel(TheoDoiNhanVienBanHang_Param param);
        Task<List<KeyValuePair<int, string>>> GetListSanPham();
        Task<List<KeyValuePair<int, string>>> GetListNhanVien();
    }
}