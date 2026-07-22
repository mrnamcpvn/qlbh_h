using API._Services.Interfaces;
using API.DTOs.Maintain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class CongNoController : APIController
    {
        private readonly I_CongNo _service;

        public CongNoController(I_CongNo service)
        {
            _service = service;
        }

        [HttpGet("GetSummary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _service.GetSummary();
            return Ok(result);
        }

        [HttpGet("GetDetail")]
        public async Task<IActionResult> GetDetail(int khachHangID)
        {
            var result = await _service.GetDetail(khachHangID);
            return Ok(result);
        }

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData([FromQuery] CongNoFilterDTO filter)
        {
            var result = await _service.GetData(filter ?? new CongNoFilterDTO());
            return Ok(result);
        }

        [HttpGet("GetCustomerDebtInfo")]
        public async Task<IActionResult> GetCustomerDebtInfo(int khachHangID)
        {
            var result = await _service.GetCustomerDebtInfo(khachHangID);
            return Ok(result);
        }
    }
}
