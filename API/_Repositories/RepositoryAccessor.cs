using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API._Repositories
{
    public class RepositoryAccessor<_DBContext> : RepositoryAccessorBase<_DBContext>, IRepositoryAccessor where _DBContext : DbContext
    {
        public RepositoryAccessor(_DBContext dbContext)
        {
            _context = dbContext;
             KhachHang = new Repository<KhachHang, _DBContext>(_context);
            SanPham = new Repository<SanPham, _DBContext>(_context);
            DonHang = new Repository<DonHang, _DBContext>(_context);
            NguoiDung = new Repository<NguoiDung, _DBContext>(_context);
            ChiTietDonHang = new Repository<ChiTietDonHang, _DBContext>(_context);
            NhanVien = new Repository<NhanVien, _DBContext>(_context);
            CuaHang = new Repository<CuaHang, _DBContext>(_context);
            NhaCungCap = new Repository<NhaCungCap, _DBContext>(_context);
        }
        public IRepository<KhachHang> KhachHang { get; set; }
        public IRepository<SanPham> SanPham { get; set; }
        public IRepository<DonHang> DonHang { get; set; }
        public IRepository<NguoiDung> NguoiDung { get; set; }
        public IRepository<ChiTietDonHang> ChiTietDonHang { get; set; }
        public IRepository<NhanVien> NhanVien { get; set; }
        public IRepository<CuaHang> CuaHang { get; }
        public IRepository<NhaCungCap> NhaCungCap { get; }
    }
}