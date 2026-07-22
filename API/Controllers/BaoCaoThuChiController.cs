using API._Services.Interfaces;
using API.DTOs.Maintain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BaoCaoThuChiController : APIController
    {
        private readonly I_BaoCaoThuChi _service;

        public BaoCaoThuChiController(I_BaoCaoThuChi service)
        {
            _service = service;
        }

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData([FromQuery] BaoCaoThuChiParam param)
        {
            var result = await _service.GetData(param);
            return Ok(result);
        }

        [HttpGet("Excel")]
        public async Task<IActionResult> Excel([FromQuery] BaoCaoThuChiParam param)
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
