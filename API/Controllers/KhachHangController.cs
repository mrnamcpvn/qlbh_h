using API._Services.Interfaces;
using API.Helpers.Params;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class KhachHangController : APIController
    {
        private readonly I_KhachHang _service;

        public KhachHangController(I_KhachHang service)
        {
            _service = service;
        }

        [HttpGet("GetDataPagination")]
        public async Task<IActionResult> GetDataPagination([FromQuery] PaginationParams pagination, string name)
        {
            var result = await _service.GetDataPagination(pagination, name);
            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(KhachHang model)
        {
            var result = await _service.Create(model);
            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(KhachHang model)
        {
            var result = await _service.Update(model);
            return Ok(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }
    }
}