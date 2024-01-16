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
    public class S_CalculateWages : I_CalculateWages
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_CalculateWages(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<ChamCongDTO>> GetDataPagination(PaginationParams pagination, string fromDate, string toDate)
        {
            var predicateUser = PredicateBuilder.New<ChamCong>(true);

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                predicateUser.And(x => Convert.ToDateTime(fromDate) <= x.Date && x.Date <= Convert.ToDateTime(toDate));
            }
            var data = _repoAccessor.ChamCong.FindAll(predicateUser)
                .Join(_repoAccessor.NguoiLD.FindAll(),
                    x => x.ID_NLD,
                    y => y.ID,
                    (x, y) => new { chamCong = x, nld = y }
                ).Join(_repoAccessor.CongDoan.FindAll(),
                    x => x.chamCong.ID_CD,
                    y => y.ID,
                    (x, y) => new { chamCong = x.chamCong, nld = x.nld, congDoan = y }
                ).Join(_repoAccessor.MaHang.FindAll(),
                    x => x.congDoan.IDMaHang,
                    y => y.ID,
                    (x, y) => new { chamCong = x.chamCong, nld = x.nld, congDoan = x.congDoan, maHang = y }
                ).Select(x => new ChamCongDTO
                {
                    ID = x.chamCong.ID,
                    ID_NLD = x.chamCong.ID_NLD,
                    NLD = x.nld.Name,
                    ID_CD = x.chamCong.ID_CD,
                    CD_Name = x.congDoan.Name,
                    MH_Name = x.maHang.Name,
                    Date = x.chamCong.Date,
                    Quantity = x.chamCong.Quantity,
                }).AsNoTracking().OrderBy(x => x.MH_Name).ThenBy(x => x.Date);
            var result = await PaginationUtility<ChamCongDTO>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(ChamCong model)
        {
            _repoAccessor.ChamCong.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Update(ChamCong model)
        {
            _repoAccessor.ChamCong.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var cong = await _repoAccessor.ChamCong.FindSingle(x => x.ID == id);
            if (cong != null)
            {
                _repoAccessor.ChamCong.Remove(cong);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<ChamCongDTO> GetDetail(int id)
        {
            return await _repoAccessor.ChamCong.FindAll(x => x.ID == id)
                .Join(_repoAccessor.CongDoan.FindAll(),
                    x => x.ID_CD,
                    y => y.ID,
                    (x, y) => new { chamcong = x, congdoan = y }
                ).Select(x => new ChamCongDTO {
                    ID = x.chamcong.ID,
                    Date = x.chamcong.Date,
                    ID_NLD = x.chamcong.ID_NLD,
                    Quantity = x.chamcong.Quantity,
                    ID_CD = x.chamcong.ID_CD,
                    IDMaHang = x.congdoan.IDMaHang.Value
                }).FirstOrDefaultAsync();
        }
    }
}