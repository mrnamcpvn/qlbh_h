using API.DTOs.Maintain;
using API.Helpers.Params;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_LichSuHangHoa
    {
        Task<LichSuHangHoaData> GetDataPagination(PaginationParams pagination, LichSuHangHoaParam param);
        Task<OperationResult> Excel(LichSuHangHoaParam param);
        Task<List<KeyValuePair<int, string>>> GetListSanPham();
        Task<List<KeyValuePair<int, string>>> GetListKhachHang();
        Task<List<KeyValuePair<int, string>>> GetListNhaCungCap();
        Task<List<KeyValuePair<int, string>>> GetListNhanVien();
    }
}
