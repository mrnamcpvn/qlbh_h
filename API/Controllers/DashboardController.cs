using API._Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class DashboardController : APIController
    {
        private readonly I_Dashboard _service;

        public DashboardController(I_Dashboard service)
        {
            _service = service;
        }

        [HttpGet("GetSummary")]
        public async Task<IActionResult> GetSummary([FromQuery] string filterType = "month")
        {
            var result = await _service.GetSummary(filterType);
            return Ok(result);
        }

        [HttpGet("GetThuChiSummary")]
        public async Task<IActionResult> GetThuChiSummary([FromQuery] string filterType = "month")
        {
            var result = await _service.GetThuChiSummary(filterType);
            return Ok(result);
        }

        [HttpGet("GetOverdueOrders")]
        public async Task<IActionResult> GetOverdueOrders()
        {
            var result = await _service.GetOverdueOrders();
            return Ok(result);
        }
    }
}
