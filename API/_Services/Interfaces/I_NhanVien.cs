using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_NhanVien
    {
        Task<PaginationUtility<NhanVien>> GetDataPagination(PaginationParam pagination, string name);
        Task<bool> Create(NhanVien model);
        Task<bool> Update(NhanVien model);
        Task<bool> Delete(int id);
        Task<List<NhanVien>> GetAll();
    }
}