using Agriculture.Services.Queries;
using Agriculture.Services.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRAWAN_EV_API.Services.Response;
using System.ComponentModel.DataAnnotations;

namespace Agriculture.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrdersController(OrderService orderService) 
        {
            _orderService = orderService;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrdersList(int pagesize = 10 , int currentpage = 1,int status = 0,string? startDate ="" , string? endDate="")
        {
            try
            {
                var response = await _orderService.GetOrdersList(pagesize,currentpage,status,startDate,endDate);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromForm]OrderReq data)
        {
            try
            {
                var response = await _orderService.Create(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpGet("[action]/{orderId}")]
        public async Task<IActionResult> GetOrderDetail([Required] int orderId)
        {
            try
            {
                var response = await _orderService.GetOrderDetail(orderId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ChangeStatus([Required] int id , int status)
        {
            try
            {
                var response = await _orderService.ChangeStatus(id , status);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
