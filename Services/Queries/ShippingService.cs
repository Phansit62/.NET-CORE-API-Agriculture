using Agriculture.Services.Request;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Services.Response;

namespace Agriculture.Services.Queries
{
    public class ShippingService
    {
        private readonly DataContext _dataContext;
        public ShippingService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ResponseMessage> Create(ShippingReq data)
        {
             await _dataContext.Shippings.AddAsync(new Shipping
            {
                OrdersId = data.OrdersId,
                ShippingDate = DateTime.Now,
                ShoppingType = data.ShoppingType,
                TrackingNumber = data.TrackingNumber,
            });
            int save = await _dataContext.SaveChangesAsync();
            if(save > 0 )
            {
                var order = await _dataContext.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.OrdersId);
                if( order != null)
                {
                    order.OrderStatus = 3;
                    _dataContext.Entry(order).State = EntityState.Modified;
                    await _dataContext.SaveChangesAsync();
                }
                return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }
    }
}
