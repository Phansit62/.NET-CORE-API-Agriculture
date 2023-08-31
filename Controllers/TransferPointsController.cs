using Agriculture.Services.Queries;
using Agriculture.Services.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRAWAN_EV_API.Services.Response;
using System.ComponentModel.DataAnnotations;

namespace Agriculture.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferPointsController : ControllerBase
    {
        private readonly TransferPointService _transferPointService;

        public TransferPointsController(TransferPointService transferPointService)
        {
            _transferPointService = transferPointService;
        }

        [HttpGet("[action]/{userId}")]
        public async Task<IActionResult> GetTransferPointList([Required]string userId ="")
        {
            try
            {
                var response = await _transferPointService.GetTransferPointList(userId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(TransferPointReq data)
        {
            try
            {
                var response = await _transferPointService.Create(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
