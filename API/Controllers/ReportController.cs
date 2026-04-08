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

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData([FromQuery] ReportParam param)
        {
            return Ok(await _service.GetData(param));
        }

        [HttpGet("Excel")]
        public async Task<IActionResult> Excel([FromQuery] ReportParam param)
        {
            return Ok(await _service.Excel(param));
        }
    }
}