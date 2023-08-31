using Agriculture.Services.Request;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Services.Response;

namespace Agriculture.Services.Queries
{
    public class ReviewsService
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReviewsService(DataContext context ,IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseData> GetReviewsList(int productId)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Profile/";
            var query = await _context.Reviews.Where(x => x.ProductsId == productId).Include(x=>x.Users).Select(s=> new
            {
                s.Id,
                s.rating,
                s.Comment,
                name = s.Users.FirstName,
                image = url+s.Users.ImageProfile
            }).ToListAsync();
            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        public async Task<ResponseData> GetReviewDetail(int orderId)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/ProductImage/";
            var query = await _context.OrderDetails.Include(x => x.Products).ThenInclude(i=>i.Images).Where(x => x.OrdersId == orderId).Select(s=> new
            {
                productsId = s.ProductsId,
                Name = s.Products.Name,
                Image = s.Products.Images.Count > 0 ? url + s.Products.Images[0].Path : "",
            }).ToListAsync();

            if(query.Count > 0)
            {
                return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
            }
            return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "UnSuccess", Data = { } };
        }

        public async Task<ResponseMessage> CreateReview(List<ReviewsReq> data)
        {
            if (data != null && data.Count > 0)
            {
                foreach (var req in data)
                {
                    var productCheck = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.ProductsId);
                    if (productCheck != null)
                    {
                        await _context.Reviews.AddRangeAsync(new
                        Reviews
                        {
                            ProductsId = req.ProductsId,
                            Comment = req.Comment,
                            rating = req.rating,
                            UsersId = req.UsersId,
                            CreatedAt = DateTime.Now,
                        });
                    }
                   
                }
                int save = await _context.SaveChangesAsync();
                if (save == data.Count)
                {
                    var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data[0].UsersId);
                    var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data[0].OrderId);
                    if (user != null)
                    {
                        user.Points += (5 * data.Count);
                        _context.Entry(user).State = EntityState.Modified;
                    }
                    if(order != null)
                    {
                        order.IsReview = 1;
                        _context.Entry(order).State = EntityState.Modified;
                    }
                    await _context.SaveChangesAsync();

                    return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                }
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }
    }
}
