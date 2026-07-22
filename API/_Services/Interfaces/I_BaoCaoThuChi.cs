using API.DTOs.Maintain;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
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
