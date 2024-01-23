using API._Services.Interfaces;
using API.Helpers.Params;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class DonHangController : APIController
    {
        private readonly I_DonHang _service;

        public DonHangController(I_DonHang service)
        {
            _service = service;
        }

       [HttpGet("GetMuaHangPagination")]
        public async Task<IActionResult> GetMuaHangPagination([FromQuery] PaginationParams pagination, string fromDate, string toDate)
        {
            var result = await _service.GetDataPagination(pagination, fromDate, toDate, 1);
            return Ok(result);
        }

       // [HttpPost("Create")]
        // public async Task<IActionResult> Create(ChamCong model)
        // {
        //     var result = await _service.Create(model);
        //     return Ok(result);
        // }

        // [HttpDelete("Delete")]
        // public async Task<IActionResult> Delete(int id)
        // {
        //     var result = await _service.Delete(id);
        //     return Ok(result);
        // }

        // [HttpPut("Update")]
        // public async Task<IActionResult> Update(ChamCong model)
        // {
        //     var result = await _service.Update(model);
        //     return Ok(result);
        // }

        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.GetDetail(id);
            return Ok(result);
        }
    }
}