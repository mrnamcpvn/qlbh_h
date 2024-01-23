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
    public class S_SanPham : I_SanPham
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_SanPham(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }

        public async Task<PaginationUtility<SanPham>> GetDataPagination(PaginationParams pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<SanPham>(true);

            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Ten.Contains(name) || x.MaSP.Contains(name));
            }
            var data = _repoAccessor.SanPham.FindAll(predicateUser).OrderBy(x => x.MaSP);
            var result = await PaginationUtility<SanPham>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        // public async Task<PaginationUtility<SanPhamDTO>> GetDataPagination(PaginationParams pagination, string name)
        // {
        //     var predicateUser = PredicateBuilder.New<SanPham>(true);

        //     if (!string.IsNullOrEmpty(name))
        //     {
        //         predicateUser.And(x => x.Name.Trim().Contains(name));
        //     }
        //     var data = _repoAccessor.SanPham.FindAll(predicateUser)
        //         .Join(_repoAccessor.MaHang.FindAll(),
        //             x => x.IDMaHang,
        //             y => y.ID,
        //             (x, y) => new { SanPham = x, maHang = y }
        //         ).Select(x => new SanPhamDTO
        //         {
        //             ID = x.SanPham.ID,
        //             IDMaHang = x.SanPham.IDMaHang,
        //             MaHang = x.maHang.Name,
        //             Money = x.SanPham.Money,
        //             Name = x.SanPham.Name
        //         }).OrderBy(x => x.MaHang);
        //     var result = await PaginationUtility<SanPhamDTO>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
        //     return result;
        // }

        public async Task<bool> Create(SanPham model)
        {
            _repoAccessor.SanPham.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var cd = await _repoAccessor.SanPham.FindSingle(x => x.ID == id);
            if (cd != null)
            {
                _repoAccessor.SanPham.Remove(cd);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Update(SanPham model)
        {
            _repoAccessor.SanPham.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<List<KeyValuePair<int, string>>> GetAll()
        {
            var data = await _repoAccessor.SanPham.FindAll().Select(x => new KeyValuePair<int, string>(x.ID, x.Ten)).ToListAsync();
            return data;
        }

        // public async Task<List<KeyValuePair<int, string>>> GetAllByCommodityCodeId(int id)
        // {
        //     var data = await _repoAccessor.SanPham.FindAll(x => x.IDMaHang == id).Select(x => new KeyValuePair<int, string>(x.ID, x.Name)).ToListAsync();
        //     return data;
        // }
    }
}