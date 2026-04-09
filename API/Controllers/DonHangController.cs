using API._Services.Interfaces;
using API.DTOs.Maintain;
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

       [HttpGet("GetDonHangPagination")]
        public async Task<IActionResult> GetDonHangPagination([FromQuery] DonHangRequestDTO filter)
        {
            var result = await _service.GetDataPagination(filter);
            return Ok(result);
        }
        [HttpGet("ExcelExport")]
        public async Task<ActionResult> ExcelExport([FromQuery] DonHangRequestDTO filter)
        {
            var result = await _service.DownloadExcel(filter);
            return Ok(result);
        }


       [HttpPost("Create")]
        public async Task<IActionResult> Create(DonHangDTO model)
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

        [HttpDelete("DeleteItem")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var result = await _service.DeleteItem(id);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(DonHangDTO model)
        {
            var result = await _service.Update(model);
            return Ok(result);
        }

        [HttpPost("UpdatePayment")]
        public async Task<IActionResult> UpdatePayment(DonHang model)
        {
            var result = await _service.UpdatePayment(model);
            return Ok(result);
        }

        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _service.GetDetail(id);
            return Ok(result);
        }
    }
}