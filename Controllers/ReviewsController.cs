using Agriculture.Services.Queries;
using Agriculture.Services.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRAWAN_EV_API.Services.Response;

namespace Agriculture.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewsService _reviewsService;

        public ReviewsController(ReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }

        [HttpGet("[action]/{productId}")]
        public async Task<IActionResult> GetReviewsList(int productId)
        {
            try
            {
                var response = await _reviewsService.GetReviewsList(productId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400 , Message = ex.Message});
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetReviewDetail(int id)
        {
            try
            {
                var response = await _reviewsService.GetReviewDetail(id);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateReview(List<ReviewsReq> data )
        {
            try
            {
                var response = await _reviewsService.CreateReview(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
