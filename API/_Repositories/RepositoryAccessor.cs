
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
            NguoiLD = new Repository<NguoiLaoDong>(_dbContext);
            CongDoan = new Repository<CongDoan>(_dbContext);
            ChamCong = new Repository<ChamCong>(_dbContext);
            MaHang = new Repository<MaHang>(_dbContext);
            NguoiDung = new Repository<NguoiDung>(_dbContext);
        }

        public IRepository<NguoiLaoDong> NguoiLD { get; set; }
        public IRepository<CongDoan> CongDoan { get; set; }
        public IRepository<ChamCong> ChamCong { get; set; }
        public IRepository<MaHang> MaHang { get; set; }
        public IRepository<NguoiDung> NguoiDung { get; set; }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}