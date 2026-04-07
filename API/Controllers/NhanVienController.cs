using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API._Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class NhanVienController : APIController
    {
        private readonly I_NhanVien _service;
        public NhanVienController(I_NhanVien service)
        {
            _service = service;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }
    }
}