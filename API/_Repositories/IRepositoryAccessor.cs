
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
        Task<bool> Save();
    }
}