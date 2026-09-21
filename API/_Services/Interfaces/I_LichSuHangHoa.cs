using API.DTOs.Maintain;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_LichSuHangHoa
    {
        Task<LichSuHangHoaData> GetDataPagination(PaginationParam pagination, LichSuHangHoaParam param);
        Task<OperationResult> Excel(LichSuHangHoaParam param);
        Task<List<KeyValuePair<int, string>>> GetListSanPham();
        Task<List<KeyValuePair<int, string>>> GetListKhachHang();
        Task<List<KeyValuePair<int, string>>> GetListNhaCungCap();
        Task<List<KeyValuePair<int, string>>> GetListNhanVien();
    }
}
