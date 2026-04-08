using API._Repositories;
using API._Services.Interfaces;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_CuaHang : I_CuaHang
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_CuaHang(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<List<CuaHang>> GetAll()
        {
            var data = await _repoAccessor.CuaHang.FindAll().ToListAsync();
            return data;
        }
    }
}