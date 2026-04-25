using API._Services.Interfaces;
using API.DTOs.Maintain;
using API.Helpers.Params;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LichSuHangHoaController : APIController
    {
        private readonly I_LichSuHangHoa _service;

        public LichSuHangHoaController(I_LichSuHangHoa service)
        {
            _service = service;
        }

        [HttpGet("GetDataPagination")]
        public async Task<IActionResult> GetDataPagination([FromQuery] PaginationParams pagination, [FromQuery] LichSuHangHoaParam param)
        {
            var result = await _service.GetDataPagination(pagination, param);
            return Ok(result);
        }

        [HttpGet("Excel")]
        public async Task<IActionResult> Excel([FromQuery] LichSuHangHoaParam param)
        {
            return Ok(await _service.Excel(param));
        }

        [HttpGet("GetListSanPham")]
        public async Task<IActionResult> GetListSanPham()
        {
            var result = await _service.GetListSanPham();
            return Ok(result);
        }

        [HttpGet("GetListKhachHang")]
        public async Task<IActionResult> GetListKhachHang()
        {
            var result = await _service.GetListKhachHang();
            return Ok(result);
        }

        [HttpGet("GetListNhaCungCap")]
        public async Task<IActionResult> GetListNhaCungCap()
        {
            var result = await _service.GetListNhaCungCap();
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
