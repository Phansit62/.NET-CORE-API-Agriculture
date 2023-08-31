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
    public class NewsController : ControllerBase
    {
        private readonly NewsService _newsService;

        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetNews(int pagesize = 10, int currentpage = 1, string? search ="")
        {
            try
            {
                var response = await _newsService.GetNews(pagesize, currentpage, search);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetNewsList()
        {
            try
            {
                var response = await _newsService.GetNewsList();
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetNewsDetail([Required]int id)
        {
            try
            {
                var response = await _newsService.GetNewsDetail(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromForm]NewsReq data)
        {
            try
            {
                var response = await _newsService.Create(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Update([FromForm] NewsReq data)
        {
            try
            {
                var response = await _newsService.Update(data);
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
                var response = await _newsService.Delete(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
