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
        public async Task<PaginationUtility<CuaHang>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<CuaHang>(true);


            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Ten.Contains(name));
            }
            var data = _repoAccessor.CuaHang.FindAll(predicateUser);
            var result = await PaginationUtility<CuaHang>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(CuaHang model)
        {
            _repoAccessor.CuaHang.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Update(CuaHang model)
        {
            _repoAccessor.CuaHang.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var nv = await _repoAccessor.CuaHang.FindSingle(x => x.ID == id);
            if (nv != null)
            {
                _repoAccessor.CuaHang.Remove(nv);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }
        public async Task<List<CuaHang>> GetAll()
        {
            var data = await _repoAccessor.CuaHang.FindAll().ToListAsync();
            return data;
        }
    }
}