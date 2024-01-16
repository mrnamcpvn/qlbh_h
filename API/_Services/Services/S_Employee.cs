using API._Repositories;
using API._Services.Interfaces;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_Employee : I_Employee
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_Employee(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<NguoiLaoDong>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<NguoiLaoDong>(true);


            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Name.Trim().Contains(name));
            }
            var data = _repoAccessor.NguoiLD.FindAll(predicateUser);
            var result = await PaginationUtility<NguoiLaoDong>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(string name)
        {
            NguoiLaoDong model = new NguoiLaoDong();
            model.Name = name;
            _repoAccessor.NguoiLD.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Update(NguoiLaoDong model)
        {
            _repoAccessor.NguoiLD.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var nld = await _repoAccessor.NguoiLD.FindSingle(x => x.ID == id);
            if (nld != null)
            {
                _repoAccessor.NguoiLD.Remove(nld);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<List<NguoiLaoDong>> GetAll()
        {
            var data = await _repoAccessor.NguoiLD.FindAll().ToListAsync();
            return data;
        }
    }
}