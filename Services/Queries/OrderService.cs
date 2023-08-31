using Agriculture.Helpers;
using Agriculture.Models;
using Agriculture.Services.Request;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Helpers;
using PRAWAN_EV_API.Services.Response;
using static System.Net.Mime.MediaTypeNames;

namespace Agriculture.Services.Queries
{
    public class OrderService
    {
        private readonly DataContext _dataContext;
        private readonly CheckFile _checkFile;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(DataContext dataContext, CheckFile checkFile, IWebHostEnvironment hostingEnvironment , IHttpContextAccessor httpContextAccessor) { 
            _dataContext = dataContext;
            _hostingEnvironment = hostingEnvironment;
            _checkFile = checkFile;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponsePagination> GetOrdersList(int pagesize, int currentpage, int status,string startDate,string endDate)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}\\Payment\\";
            var query = await _dataContext.Orders.Include(u=>u.Users).Include(o => o.OrderDetail).ThenInclude(o => o.Products).Select(s => new
            {
                Id = s.Id,
                OrderDetail = s.OrderDetail.Select(x=> new
                {
                    x.Products.Name,
                    x.Products.Category,
                    x.Products.Unit,
                    x.Products.Price,
                    x.Quantity,
                    x.TotalPrice,
                }).ToList(),
                User = s.Users,
                s.OrderStatus,
                OrderDate=  s.OrderDate,
                Payment = _dataContext.Payments.Select(x=> new
                {
                    x.Id,
                    Image = url + x.Image,
                    x.Amount,
                    x.PaymentMethod,
                    x.OrdersId
                }).FirstOrDefault(x=>x.OrdersId == s.Id),
                s.IsReview
            }).Where(x=> status != 0 ? x.OrderStatus == status : true)
             .Where(x => !string.IsNullOrEmpty(startDate) ? x.OrderDate.Value.Date >= Convert.ToDateTime(startDate).Date : true)
              .Where(x => !string.IsNullOrEmpty(endDate) ? x.OrderDate.Value.Date <= Convert.ToDateTime(endDate).Date : true)
            .ToListAsync();

            Pagination pagination = new Pagination(query.ToList<dynamic>(), currentpage, pagesize);

            return new ResponsePagination { StatusCode = 200, TaskStatus = true, Message = "สำเร็จ", Pagin = new { Currentpage = currentpage, Pagesize = pagesize, Totalrows = pagination.TotalRow, Totalpages = pagination.TotalPage }, Data = pagination.Values };
        }

        public async Task<ResponseMessage> Create (OrderReq data)
        {
            if (data.Products.Count > 0)
            {
                var order = _dataContext.Orders.Add(new Orders
                {
                    OrderDate = DateTime.Now,
                    AddressId = data.AddressId,
                    OrderStatus = 1,
                    UsersId = data.UserId,
                    TotalPrice = data.TotalPrice,
                    IsReview = 0
                });
                int saveOrder = await _dataContext.SaveChangesAsync();
                if(saveOrder > 0)
                {
                    foreach (var product in data.Products)
                    {
                        _dataContext.OrderDetails.AddRange(new OrderDetail
                        {
                            OrdersId = order.Entity.Id,
                            ProductsId = product.ProductId,
                            Quantity = product.Quantity,
                            TotalPrice = product.Price * product.Quantity
                        });
                        var chkProduct = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == product.ProductId);
                        chkProduct.Stock -= product.Quantity;
                        _dataContext.UpdateRange(chkProduct);
                    }
                    int saveOrderDetail = await _dataContext.SaveChangesAsync();
                    if(saveOrderDetail > 0)
                    {
                        string image = string.Empty;
                        if (data.Upload != null)
                        {

                            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Payment");
                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);

                            if (!_checkFile.IsImageFile(data.Upload))
                            {
                                return new ResponseMessage { Message = "กรุณาอัพโหลดไฟล์รูปภาพ เท่านั้น!!", StatusCode = 200, TaskStatus = false };
                            }

                            string typefile = Path.GetExtension(data.Upload.FileName);
                            string uniqueFileName = Guid.NewGuid().ToString() + typefile;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await data.Upload.CopyToAsync(fileStream);
                            }
                            image = uniqueFileName;
                        }
                        await _dataContext.Payments.AddAsync(new Payment
                        {
                            OrdersId = order.Entity.Id,
                            CouponsId = data.CouponId == 0 ? null : data.CouponId,
                            Amount = data.TotalPrice,
                            PaymentMethod = data.PaymentMethod,
                            Status = data.PaymentMethod == 1 ? 1 : 2,
                            TransportationCost = 0,
                            Image = image,
                            PaymentDate = DateTime.Now,
                        });
                        int savePayment = await _dataContext.SaveChangesAsync();
                        if(savePayment > 0) 
                        {
                            if(data.CouponId != null)
                            {
                                var chkCoupon = await _dataContext.CouponUsers.AsNoTracking().FirstOrDefaultAsync(x => x.CouponsId == data.CouponId && x.UsersId == data.UserId);

                                if (chkCoupon != null)
                                {
                                    chkCoupon.IsUsed = 1;
                                    _dataContext.Entry(chkCoupon).State = EntityState.Modified;
                                    int save = await _dataContext.SaveChangesAsync();
                                    if (save > 0)
                                    {
                                        return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };

                                    }
                                }
                            }

                            return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                        }
                    }
                }
            }

            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }

        public async Task<ResponseData> GetOrderDetail(int orderId)
        {
            var query = await _dataContext.Payments.Include(o => o.Orders).ThenInclude(i => i.OrderDetail).ThenInclude(x=>x.Products)
                .Select(s => new
                {
                    s.OrdersId,
                    OrderDate = s.Orders.OrderDate,
                    OrderStatus = s.Orders.OrderStatus,
                    TotalPrice = s.Orders.TotalPrice,
                    OrderDetail = s.Orders.OrderDetail.ToList(),
                    s.PaymentMethod ,
                    s.Amount,
                    s.TransportationCost,
                    CouponsId = s.CouponsId ?? 0
                }).AsNoTracking().FirstOrDefaultAsync(x => x.OrdersId == orderId);
            if(query == null)
            {
                return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "UnSuccess", Data = { } };
            }

            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        public async Task<ResponseMessage> ChangeStatus(int id ,int status)
        {
            var chk = await _dataContext.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(chk == null)
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูล" };
            }
            chk.OrderStatus = status;
            _dataContext.Entry(chk).State = EntityState.Modified;
            int save = await _dataContext.SaveChangesAsync();
            if(save > 0)
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }
    }
}
