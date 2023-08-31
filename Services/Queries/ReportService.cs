using Agriculture.Data;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Services.Response;
using System;
using System.Globalization;
using System.Linq;

namespace Agriculture.Services.Queries
{
    public class ReportService
    {
        private readonly DataContext _context;
        public ReportService(DataContext context) 
        {
            _context = context;
        }
        private class barChart
        {
            public string Label { get; set; }
            public float Value { get; set; }
        }

        public async Task<ResponseData> DashboardMobile(int year, int type)
        {
            var user = _context.Users.Count(x=>x.Role == 1 && x.Create_At.Value.Year == year);
            var order = _context.Orders.Count(x=>x.OrderStatus != 5 && x.OrderDate.Value.Year == year);
            var orderCancel = _context.Orders.Count(x=>x.OrderStatus == 5 && x.OrderDate.Value.Year == year);
            var payment = _context.Payments.Where(x=>x.PaymentDate.Value.Year == year).Select(s=>s.Amount).Sum();

            var data = await GetBarChartDataForMonth(year, type);


            var listData = new
            {
                Charts = data,
                Orders = order,
                ordersCancel = orderCancel,
                payments = payment,
                users = user,
            };

            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = listData };
        }

        public async Task<ResponseData> ReportOrders(int status,string startDate , string endDate)
        {
            var query = await _context.Orders.Include(u => u.Users).Include(o => o.OrderDetail).ThenInclude(o => o.Products).Select(s => new
            {
                Id = s.Id,
                OrderDetail = s.OrderDetail.Count,
                User = s.Users.FirstName,
                s.OrderStatus,
                OrderDate = s.OrderDate.Value.Date,
                Payment = _context.Payments.Select(x => new
                {
                    x.Id,
                    x.Amount,
                    x.PaymentMethod,
                    x.OrdersId
                }).FirstOrDefault(x => x.OrdersId == s.Id)
            }).Where(x => status != 0 ? x.OrderStatus == status : true)
              .Where(x => !string.IsNullOrEmpty(startDate) ? x.OrderDate >= Convert.ToDateTime(startDate) : true)
              .Where(x => !string.IsNullOrEmpty(endDate) ? x.OrderDate <= Convert.ToDateTime(endDate) : true)
              .ToListAsync();
            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        public async Task<ResponseData> ReportIncome(int year , int month)
        {
            var query = await _context.Payments.Where(x => x.Status == 2)
                .Where(x=> year != 0 ? x.PaymentDate.Value.Date.Year == year : true)
                .Where(x=> month != 0 ? x.PaymentDate.Value.Date.Month == month : true)
                .ToListAsync();

            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        public async Task<ResponseData> ReportOrderCategory(int year) 
        {
            var query = await _context.OrderDetails.Include(x=>x.Orders)
                .Where(x=>year != 0 ? x.Orders.OrderDate.Value.Date.Year == year : true)
                .Include(p => p.Products).GroupBy(g => g.Products.Category).Select(s => new
                {
                    Category = s.Key,
                    CountOrder = s.Count()
                }).ToListAsync();

            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        private async Task<List<barChart>> GetBarChartDataForMonth(int year,int type)
        {
            List<barChart> barCharts = new List<barChart>();
            CultureInfo culture = new CultureInfo("th-TH");
            var calendar = CultureInfo.CurrentCulture.Calendar;

            IQueryable<dynamic> query = null;
            switch (type)
            {
                case 1:
                    query = _context.Users.Select(x=> new
                    {
                        CreatedAt = x.Create_At.Value.Date
                    });
                    break;
                case 2:
                    query = _context.Orders.Where(x=>x.OrderStatus != 5).Select(x=> new
                    {
                        CreatedAt = x.OrderDate.Value.Date
                    });
                    break;
                case 3:
                    query = _context.Payments.Select(x=> new {
                        CreatedAt = x.PaymentDate.Value.Date,
                        Amount = x.Amount
                    });
                    break;
                case 4:
                    query = _context.Orders.Where(x=>x.OrderStatus == 5).Select(x=> new
                    {
                        CreatedAt = x.OrderDate.Value.Date
                    });
                    break;
            }
            var queryTolist = await query.ToListAsync();
            for (int i = 1; i <= 12; i++)
            {
                var data = type == 3 ? queryTolist.Where(x => x.CreatedAt.Month == i && x.CreatedAt.Year == year).Select(x => (int)x.Amount).Sum() : queryTolist.Where(x => x.CreatedAt.Month == i && x.CreatedAt.Year == year).Count();
                barCharts.Add(new barChart
                {
                    Label = new DateTime(DateTime.Now.Year, i, 1).ToString("MMM", culture),
                    Value =   data
                });
            }
            return barCharts;
        }
    }
}
