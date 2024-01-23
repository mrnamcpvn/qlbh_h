using API._Repositories;
using API._Services.Interfaces;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_KhachHang : I_KhachHang
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_KhachHang(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<KhachHang>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<KhachHang>(true);


            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Ten.Trim().Contains(name));
            }
            var data = _repoAccessor.KhachHang.FindAll(predicateUser);
            var result = await PaginationUtility<KhachHang>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(KhachHang model)
        {
            _repoAccessor.KhachHang.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Update(KhachHang model)
        {
            _repoAccessor.KhachHang.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var nld = await _repoAccessor.KhachHang.FindSingle(x => x.ID == id);
            if (nld != null)
            {
                _repoAccessor.KhachHang.Remove(nld);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<List<KhachHang>> GetAll()
        {
            var data = await _repoAccessor.KhachHang.FindAll().ToListAsync();
            return data;
        }
    }
}