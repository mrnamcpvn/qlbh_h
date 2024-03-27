using API._Services.Interfaces;
using API.DTOs.Report;
using Microsoft.AspNetCore.Mvc;
using SD3_API.Helpers.Utilities;

namespace API.Controllers
{
    public class ReportController : APIController
    {
        private readonly I_Report _service;

        public ReportController(I_Report service)
        {
            _service = service;
        }

        [HttpGet("GetDataPagination")]
        public async Task<IActionResult> GetDataPagination([FromQuery] PaginationParam pagination, [FromQuery] ReportParam param)
        {
            return Ok(await _service.GetDataPagination(pagination, param));
        }

        // [HttpGet("ExportExcel")]
        // public async Task<IActionResult> ExportExcel([FromQuery] PaginationParam pagination, [FromQuery] ReportParam param)
        // {
        //     byte[] result = await _service.ExportExcel(pagination, param, false);
        //     return File(result, "application/xlsx");
        // }
    }
}