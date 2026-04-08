using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helpers.Params;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TheoDoiNhanVienBanHangController : APIController
    {
        private readonly I_TheoDoiNhanVienBanHang _service;

        public TheoDoiNhanVienBanHangController(I_TheoDoiNhanVienBanHang service)
        {
            _service = service;
        }

        [HttpGet("GetDataPagination")]
        public async Task<IActionResult> GetDataPagination([FromQuery] PaginationParams pagination, [FromQuery] TheoDoiNhanVienBanHang_Param param)
        {
            var result = await _service.GetDataPagination(pagination, param);
            return Ok(result);
        }
        [HttpGet("Excel")]
        public async Task<IActionResult> Excel([FromQuery] TheoDoiNhanVienBanHang_Param param)
        {
            return Ok(await _service.Excel(param));
        }
        [HttpGet("GetListSanPham")]
        public async Task<IActionResult> GetListSanPham()
        {
            var result = await _service.GetListSanPham();
            return Ok(result);
        }
        [HttpGet("GetListNhanVien")]
        public async Task<IActionResult> GetListNhanVien()
        {
            var result = await _service.GetListNhanVien();
            return Ok(result);
        }
    }
}