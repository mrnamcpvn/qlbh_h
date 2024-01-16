using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

            Database.SetCommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
        }
        
        
        public virtual DbSet<NguoiLaoDong> NguoiLaoDong {get;set;}
        public virtual DbSet<CongDoan> CongDoan {get;set;}
        public virtual DbSet<ChamCong> ChamCong {get;set;}
        public virtual DbSet<MaHang> MaHang { get; set; }
        public virtual DbSet<NguoiDung> NguoiDung { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}