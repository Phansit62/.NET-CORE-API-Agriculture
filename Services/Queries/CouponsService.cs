using Agriculture.Services.Request;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Helpers;
using PRAWAN_EV_API.Services.Response;
using System.Collections.Generic;

namespace Agriculture.Services.Queries
{
    public class CouponsService
    {
        private readonly DataContext _dataContext;
        public CouponsService(DataContext dataContext) 
        { 
            _dataContext = dataContext;
        }

        public async Task<ResponsePagination> GetCouponList(int pagesize , int currentpage, string search)
        {
            var query = await _dataContext.Coupons.Select(s=> new
            {
                Id = s.Id,
                Name = s.Name,
                Points = s.Points,
                Limit = s.limit,
                Discount =  s.Discount ,
                ExpiredAt = s.ExpiredAt

            }).Where(x=> x.Limit > 0)
              .Where(x=> x.ExpiredAt.Value.Date >= DateTime.Now.Date)
              .Where(x=> !string.IsNullOrEmpty(search) ? x.Name.Contains(search) : true).ToListAsync<dynamic>();
            Pagination pagination = new Pagination(query, currentpage, pagesize);
            return new ResponsePagination { StatusCode = 200, TaskStatus = true, Message = "สำเร็จ", Pagin = new { Currentpage = currentpage, Pagesize = pagesize, Totalrows = pagination.TotalRow, Totalpages = pagination.TotalPage }, Data = pagination.Values };
        }

        public async Task<ResponseData> GetCouponDetail(int id)
        {
            var query = await _dataContext.Coupons.Select(s=> new
            {
                s.Id,
                s.Name,
                s.limit,
                s.Points,
                s.Discount,
                ExpiredAt = s.ExpiredAt.Value.Date.ToString("yyyy-MM-dd"),
            }).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(query == null)
            {
                return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูล", Data =  { } };
            }

            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        public async Task<ResponseMessage> Create(CouponReq data)
        {
            await _dataContext.Coupons.AddAsync(new Coupons
            {
                Name = data.Name,
                limit = data.limit,
                Points = data.Points,
                Discount = data.Discount,
                ExpiredAt = data.ExpiredAt,
                CreatedAt = DateTime.Now
            });
            int save = await _dataContext.SaveChangesAsync();
            if(save >0)
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };

            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }

        public async Task<ResponseMessage> Update(CouponReq data)
        {
            var chk = await _dataContext.Coupons.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == data.Id);
            if(chk != null)
            {
                chk.Name = data.Name;
                chk.limit = data.limit;
                chk.Points = data.Points;
                chk.Discount = data.Discount;
                chk.ExpiredAt = data.ExpiredAt;
                int save = await _dataContext.SaveChangesAsync();
                if (save > 0)
                {
                    return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                }
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }

        public async Task<ResponseMessage> Delete(int id)
        {
            var query = await _dataContext.Coupons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (query == null)
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูล" };
            }
            _dataContext.Coupons.Remove(query);
            int save = await _dataContext.SaveChangesAsync();
            if(save > 0) 
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }
    }
}
