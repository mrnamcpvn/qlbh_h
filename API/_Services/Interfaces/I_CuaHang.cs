using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API._Services.Interfaces
{
    public interface I_CuaHang
    {
        Task<List<CuaHang>> GetAll();
    }
}