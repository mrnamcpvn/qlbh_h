using API._Services.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CuaHangController : APIController
    {
        private readonly I_CuaHang _service;

        public CuaHangController(I_CuaHang service)
        {
            _service = service;
        }

        [HttpGet("GetFirst")]
        public async Task<IActionResult> GetFirst()
        {
            var result = await _service.GetFirst();
            return Ok(result);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save(CuaHang model)
        {
            var result = await _service.Save(model);
            return Ok(result);
        }
    }
}