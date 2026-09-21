using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API._Services.Interfaces
{
    [DependencyInjection(ServiceLifetime.Scoped)]
    public interface I_CuaHang
    {
        Task<CuaHang> GetFirst();
        Task<bool> Save(CuaHang model);
    }
}