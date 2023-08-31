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
    public class CouponController : ControllerBase
    {
        private readonly CouponsService _couponsService;
       public CouponController(CouponsService couponsService) 
        {
            _couponsService= couponsService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCouponList(int pagesize = 10 , int currentpage = 1,string? search = "")
        {
            try
            {
                var response = await _couponsService.GetCouponList(pagesize,currentpage, search);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
        
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetCouponDetail([Required] int id)
        {
            try
            {
                var response = await _couponsService.GetCouponDetail(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
        
        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CouponReq data)
        {
            try
            {
                var response = await _couponsService.Create(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Update(CouponReq data)
        {
            try
            {
                var response = await _couponsService.Update(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete([Required] int id)
        {
            try
            {
                var response = await _couponsService.Delete(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
