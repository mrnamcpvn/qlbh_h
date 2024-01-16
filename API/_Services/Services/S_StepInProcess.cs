using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_StepInProcess : I_StepInProcess
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_StepInProcess(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<CongDoanDTO>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<CongDoan>(true);

            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Name.Trim().Contains(name));
            }
            var data = _repoAccessor.CongDoan.FindAll(predicateUser)
                .Join(_repoAccessor.MaHang.FindAll(),
                    x => x.IDMaHang,
                    y => y.ID,
                    (x, y) => new { congdoan = x, maHang = y }
                ).Select(x => new CongDoanDTO
                {
                    ID = x.congdoan.ID,
                    IDMaHang = x.congdoan.IDMaHang,
                    MaHang = x.maHang.Name,
                    Money = x.congdoan.Money,
                    Name = x.congdoan.Name
                }).OrderBy(x => x.MaHang);
            var result = await PaginationUtility<CongDoanDTO>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(CongDoan model)
        {
            _repoAccessor.CongDoan.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var cd = await _repoAccessor.CongDoan.FindSingle(x => x.ID == id);
            if (cd != null)
            {
                _repoAccessor.CongDoan.Remove(cd);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Update(CongDoan model)
        {
            _repoAccessor.CongDoan.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<List<KeyValuePair<int, string>>> GetAll()
        {
            var data = await _repoAccessor.CongDoan.FindAll().Select(x => new KeyValuePair<int, string>(x.ID, x.Name)).ToListAsync();
            return data;
        }

        public async Task<List<KeyValuePair<int, string>>> GetAllByCommodityCodeId(int id)
        {
            var data = await _repoAccessor.CongDoan.FindAll(x => x.IDMaHang == id).Select(x => new KeyValuePair<int, string>(x.ID, x.Name)).ToListAsync();
            return data;
        }
    }
}