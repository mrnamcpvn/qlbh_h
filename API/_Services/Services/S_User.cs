using API._Repositories;
using API._Services.Interfaces;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_User : I_User
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_User(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<NguoiDung>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<NguoiDung>(true);

            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.HoTen.Trim().Contains(name));
            }
            var data = _repoAccessor.NguoiDung.FindAll(predicateUser);
            var result = await PaginationUtility<NguoiDung>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(NguoiDung model)
        {
            _repoAccessor.NguoiDung.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var cd = await _repoAccessor.NguoiDung.FindSingle(x => x.ID == id);
            if (cd != null)
            {
                _repoAccessor.NguoiDung.Remove(cd);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Update(NguoiDung model)
        {
            _repoAccessor.NguoiDung.Update(model);
            return await _repoAccessor.Save();
        }
    }
}