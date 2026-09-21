using API._Repositories;
using API._Services.Interfaces;
using API.Models;
using LinqKit;
using API.Data;

namespace API._Services.Services
{
    public class S_User : BaseServices, I_User
    {

        public S_User(DBContext dbContext) : base(dbContext) { }

        public async Task<PaginationUtility<NguoiDung>> GetDataPagination(PaginationParam pagination, string name)
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
            var cd = await _repoAccessor.NguoiDung.FirstOrDefaultAsync(x => x.ID == id);
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