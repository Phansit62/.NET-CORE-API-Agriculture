using Agriculture.Services.Queries;
using Agriculture.Services.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRAWAN_EV_API.Services.Response;

namespace Agriculture.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly ShippingService _shippingService;
        public ShippingController(ShippingService shippingService) 
        {
            _shippingService = shippingService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(ShippingReq data)
        {
            try
            {
                var response = await _shippingService.Create(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
