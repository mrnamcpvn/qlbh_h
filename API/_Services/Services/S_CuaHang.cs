using API._Repositories;
using API._Services.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API._Services.Services
{
    public class S_CuaHang : BaseServices, I_CuaHang
    {

        public S_CuaHang(DBContext dbContext) : base(dbContext) { }

        public async Task<CuaHang> GetFirst()
        {
            return await _repoAccessor.CuaHang.FindAll().FirstOrDefaultAsync();
        }

        public async Task<bool> Save(CuaHang model)
        {
            var existing = await _repoAccessor.CuaHang.FindAll().FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.Ten = model.Ten;
                existing.DiaChi = model.DiaChi;
                existing.SDT = model.SDT;
                _repoAccessor.CuaHang.Update(existing);
            }
            else
            {
                _repoAccessor.CuaHang.Add(model);
            }
            return await _repoAccessor.Save();
        }
    }
}