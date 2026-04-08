using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_NhanVien
    {
        Task<PaginationUtility<NhanVien>> GetDataPagination(PaginationParams pagination, string name);
        Task<bool> Create(NhanVien model);
        Task<bool> Update(NhanVien model);
        Task<bool> Delete(int id);
        Task<List<NhanVien>> GetAll();
    }
}