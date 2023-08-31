using Agriculture.Models;
using Agriculture.Services.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRAWAN_EV_API.Services.Response;

namespace Agriculture.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;
        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DashboardMobile(int type ,int year)
        {
            try
            {
                var response = await _reportService.DashboardMobile(year , type);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ReportOrders(int status, string? startDate, string? endDate)
        {
            try
            {
                var response = await _reportService.ReportOrders(status,startDate,endDate);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> ReportIncome(int year,int month)
        {
            try
            {
                var response = await _reportService.ReportIncome(year, month);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> ReportOrderCategory(int year)
        {
            try
            {
                var response = await _reportService.ReportOrderCategory(year);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
