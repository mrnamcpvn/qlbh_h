using API._Repositories;
using API._Services.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API._Services.Services
{
    public class S_CuaHang : I_CuaHang
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_CuaHang(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

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