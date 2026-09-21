using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API._Repositories;
using API._Services.Interfaces;
using API.Models;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API._Services.Services
{
    public class S_NhanVien: BaseServices, I_NhanVien
    {

        public S_NhanVien(DBContext dbContext) : base(dbContext) { }
        public async Task<PaginationUtility<NhanVien>> GetDataPagination(PaginationParam pagination, string name)
        {
            var predicateUser = PredicateBuilder.New<NhanVien>(true);


            if (!string.IsNullOrEmpty(name))
            {
                predicateUser.And(x => x.Ten.Contains(name));
            }
            var data = _repoAccessor.NhanVien.FindAll(predicateUser);
            var result = await PaginationUtility<NhanVien>.CreateAsync(data, pagination.PageNumber, pagination.PageSize);
            return result;
        }

        public async Task<bool> Create(NhanVien model)
        {
            _repoAccessor.NhanVien.Add(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Update(NhanVien model)
        {
            _repoAccessor.NhanVien.Update(model);
            return await _repoAccessor.Save();
        }

        public async Task<bool> Delete(int id)
        {
            var nv = await _repoAccessor.NhanVien.FirstOrDefaultAsync(x => x.ID == id);
            if (nv != null)
            {
                _repoAccessor.NhanVien.Remove(nv);
                return await _repoAccessor.Save();
            }
            else
            {
                return false;
            }
        }
        public async Task<List<NhanVien>> GetAll()
        {
            var data = await _repoAccessor.NhanVien.FindAll().ToListAsync();
            return data;
        }
    }
}