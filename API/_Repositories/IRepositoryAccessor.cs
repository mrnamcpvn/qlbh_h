
using API.Models;

namespace API._Repositories
{
    public interface IRepositoryAccessor
    {
        IRepository<NguoiLaoDong> NguoiLD { get; }
        IRepository<CongDoan> CongDoan { get; }
        IRepository<ChamCong> ChamCong { get; }
        IRepository<MaHang> MaHang { get; }
        IRepository<NguoiDung> NguoiDung { get; }
        Task<bool> Save();
    }
}