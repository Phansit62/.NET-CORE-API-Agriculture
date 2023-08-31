using Agriculture.Models;
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
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productsService;

        public ProductsController(ProductsService productsService) 
        {
            _productsService = productsService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProducts(int pagesize = 10, int currentpage = 1, string? search = "")
        {
            try
            {
                var response = await _productsService.GetProducts(pagesize, currentpage, search);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpGet("[action]/{productId}")]
        public async Task<IActionResult> GetProductDetail([Required]int productId)
        {
            try
            {
                var response = await _productsService.GetProductDetail(productId);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateProduct([FromForm]ProductReq data)
        {
            try
            {
                var response = await _productsService.CreateProduct(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductReq data)
        {
            try
            {
                var response = await _productsService.UpdateProduct(data);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }

        [HttpDelete("[action]/{id}")]

        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var response = await _productsService.DeleteProduct(id);
                return StatusCode(response.StatusCode, response);
            }
            catch(Exception ex)
            {
                return BadRequest(new ResponseErrorMessages { StatusCode = 400, Message = ex.Message });
            }
        }
    }
}
