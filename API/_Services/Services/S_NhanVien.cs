using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API._Repositories;
using API._Services.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API._Services.Services
{
    public class S_NhanVien: I_NhanVien
    {
        private readonly IRepositoryAccessor _repoAccessor;

        public S_NhanVien(IRepositoryAccessor repoAccessor)
        {
            _repoAccessor = repoAccessor;
        }
        public async Task<List<NhanVien>> GetAll()
        {
            var data = await _repoAccessor.NhanVien.FindAll().ToListAsync();
            return data;
        }
    }
}