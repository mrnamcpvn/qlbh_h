
using API.Data;
using API.Models;

namespace API._Repositories
{
    public class RepositoryAccessor : IRepositoryAccessor
    {
        private DBContext _dbContext;
        public RepositoryAccessor(DBContext dbContext)
        {
            _dbContext = dbContext;
            KhachHang = new Repository<KhachHang>(_dbContext);
            SanPham = new Repository<SanPham>(_dbContext);
            DonHang = new Repository<DonHang>(_dbContext);
            NguoiDung = new Repository<NguoiDung>(_dbContext);
            ChiTietDonHang = new Repository<ChiTietDonHang>(_dbContext);
        }

        public IRepository<KhachHang> KhachHang { get; set; }
        public IRepository<SanPham> SanPham { get; set; }
        public IRepository<DonHang> DonHang { get; set; }
        public IRepository<NguoiDung> NguoiDung { get; set; }
        public IRepository<ChiTietDonHang> ChiTietDonHang { get; set; }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}