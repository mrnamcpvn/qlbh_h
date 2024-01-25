using AgileObjects.AgileMapper;
using AgileObjects.AgileMapper.Extensions;
using API._Repositories;
using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helper.Mappers;
using API.Helpers.Params;
using API.Models;
using LinqKit;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SD3_API.Helpers.Utilities;

namespace API._Services.Services
{
    public class S_DonHang : I_DonHang
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_DonHang(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<DonHang>> GetDataPagination(PaginationParams pagination,string fromDate, string toDate, int type)
        {
            var predicateUser = PredicateBuilder.New<DonHang>(true);

            if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
            {
                predicateUser.And(x => Convert.ToDateTime(fromDate) <= x.Date && x.Date <= Convert.ToDateTime(toDate));
            }
            if(type > 0) {
                predicateUser.And(x => x.Loai == type);
            }
            var donHangs = _repoAccessor.DonHang.FindAll(predicateUser).OrderByDescending(x => x.Date);
            var result = await PaginationUtility<DonHang>.CreateAsync(donHangs, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(DonHangDTO model)
        {
            _repoAccessor.DonHang.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Update(DonHang model)
        {
            _repoAccessor.DonHang.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var cong = await _repoAccessor.DonHang.FindSingle(x => x.ID == id);
            if (cong != null)
            {
                _repoAccessor.DonHang.Remove(cong);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<List<ChiTietDonHang>> GetDetail(int id)
        {
            var data = await _repoAccessor.ChiTietDonHang.FindAll(x => x.ID_DH == id).ToListAsync();
            return data;
        }
    }
}