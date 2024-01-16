using API._Repositories;
using API._Services.Interfaces;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_CommodityCode : I_CommodityCode
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_CommodityCode(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<MaHang>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<MaHang>(true);

            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Name.Trim().Contains(name));
            }
            var data = _repoAccessor.MaHang.FindAll(predicateUser).OrderBy(x => x.Name);
            var result = await PaginationUtility<MaHang>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(string name)
        {
            MaHang model = new MaHang();
            model.Name = name;
            _repoAccessor.MaHang.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Update(MaHang model)
        {
            _repoAccessor.MaHang.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var MH = await _repoAccessor.MaHang.FindSingle(x => x.ID == id);
            if (MH != null)
            {
                _repoAccessor.MaHang.Remove(MH);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<List<MaHang>> GetAll()
        {
            var data = await _repoAccessor.MaHang.FindAll().ToListAsync();
            return data;
        }
    }
}