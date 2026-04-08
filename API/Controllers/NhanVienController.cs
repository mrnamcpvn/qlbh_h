using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API._Services.Interfaces;
using API.Helpers.Params;
using API.Models;
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
        [HttpGet("GetDataPagination")]
        public async Task<IActionResult> GetDataPagination([FromQuery] PaginationParams pagination, string name)
        {
            var result = await _service.GetDataPagination(pagination, name);
            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(NhanVien model)
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
        public async Task<IActionResult> Update(NhanVien model)
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