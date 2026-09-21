using API.DTOs.Maintain;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_BaoCaoThuChi
    {
        Task<BaoCaoThuChiData> GetData(BaoCaoThuChiParam param);
        Task<OperationResult> Excel(BaoCaoThuChiParam param);
        Task<List<KeyValuePair<int, string>>> GetListSanPham();
        Task<List<KeyValuePair<int, string>>> GetListKhachHang();
        Task<List<KeyValuePair<int, string>>> GetListNhaCungCap();
        Task<List<KeyValuePair<int, string>>> GetListNhanVien();
    }
}
