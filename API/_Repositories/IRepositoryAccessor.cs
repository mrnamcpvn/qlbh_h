
using API.Models;

namespace API._Repositories
{
    public interface IRepositoryAccessor
    {
        IRepository<KhachHang> KhachHang { get; }
        IRepository<SanPham> SanPham { get; }
        IRepository<DonHang> DonHang { get; }
        IRepository<ChiTietDonHang> ChiTietDonHang { get; }
        IRepository<NguoiDung> NguoiDung { get; }
        IRepository<NhanVien> NhanVien { get; }
        IRepository<CuaHang> CuaHang { get; }
        IRepository<NhaCungCap> NhaCungCap { get; }
        Task<bool> Save();
    }
}