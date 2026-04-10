using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Params;
using API.Models;
using SD3_API.Helpers.Utilities;

namespace API._Services.Interfaces
{
    public interface I_CuaHang
    {
        Task<CuaHang> GetFirst();
        Task<bool> Save(CuaHang model);
    }
}