using Agriculture.Models;
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
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService) {
            _addressService = addressService;
        }

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAddress([Required] string id)
        {
            try
            {
                var response = await _addressService.GetAddress(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetAddressDetail([Required] int id)
        {
            try
            {
                var response = await _addressService.GetAddressDetail(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateAddress(AddressReq data)
        {
            try
            {
                var response = await _addressService.CreateAddress(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
        [Authorize]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateAddress(AddressReq data)
        {
            try
            {
                var response = await _addressService.UpdateAddress(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteAddress([Required] int id)
        {
            try
            {
                var response = await _addressService.DeleteAddress(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
