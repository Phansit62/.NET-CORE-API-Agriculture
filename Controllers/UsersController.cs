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
    public class UsersController : ControllerBase
    {
        private readonly UserService _user;

        public UsersController(UserService user) 
        {
            _user = user;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(int pagesize = 10, int currentpage = 1, string? search = "")
        {
            try
            {
                var response = await _user.GetUsers(pagesize, currentpage,search);
                return StatusCode(response.StatusCode, response);
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetUserDetail([Required]string id)
        {
            try
            {
                var response = await _user.GetUserDetail(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromForm] RegsiterReq data)
        {
            try
            {
                var response = await _user.Register(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginReq data)
        {
            try
            {
                var response = await _user.Login(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserReq data)
        {
            try
            {
                var response = await _user.UpdateUser(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("[action]/{userId}")]
        public async Task<IActionResult> GetCouponUserList([Required] string userId)
        {
            try
            {
                var response = await _user.GetCouponUserList(userId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("[action]/{userId}")]
        public async Task<IActionResult> GetHistoryOrderList([Required] string userId)
        {
            try
            {
                var response = await _user.GetHistoryOrderList(userId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("[action]/{userId}")]
        public async Task<IActionResult> GetHistoryCoupon([Required] string userId)
        {
            try
            {
                var response = await _user.GetHistoryCoupon(userId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> RedeemCoupon([Required] string userId, [Required] int couponId)
        {
            try
            {
                var response = await _user.RedeemCoupon(userId,couponId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
