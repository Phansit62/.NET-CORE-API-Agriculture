using Agriculture.Helpers;
using Agriculture.Services.Request;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Hosting.Internal;
using PRAWAN_EV_API.Helpers;
using PRAWAN_EV_API.Services.Response;
using System;
using System.Globalization;

namespace Agriculture.Services.Queries
{
    public class UserService
    {
        private readonly DataContext _dataContext;
        private readonly Token _token;
        private readonly CheckFile _checkFile;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(DataContext dataContext,Token token , CheckFile checkFile , IWebHostEnvironment hostingEnvironment , IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _token = token;
            _checkFile = checkFile;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponsePagination> GetUsers(int pagesize, int currentpage, string search)
        {
            var query = await _dataContext.Users.Where(x => x.Role == 1).Where(x=> !string.IsNullOrEmpty(search) ? x.FirstName.Contains(search) || x.LastName.Contains(search) : true).ToListAsync<dynamic>();

            Pagination pagination = new Pagination(query, currentpage, pagesize);

            return new ResponsePagination { StatusCode = 200, TaskStatus = true, Message = "สำเร็จ", Pagin = new { Currentpage = currentpage, Pagesize = pagesize, Totalrows = pagination.TotalRow, Totalpages = pagination.TotalPage }, Data = pagination.Values };
        }

        public async Task<ResponseData> GetUserDetail(string id)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Profile/";
            var couponList = await _dataContext.CouponUsers.Where(x => x.UsersId == id && x.IsUsed == 0)
                   .Select(s => s.CouponsId).ToListAsync();
            var query = _dataContext.Users.AsNoTracking().Select(s => new
            {
                s.Id,
                s.Prefix,
                ImageProfile = !string.IsNullOrEmpty(s.ImageProfile) ? $"{url}{s.ImageProfile}" : "",
                firstname = s.FirstName,
                lastname = s.LastName,
                s.Phone,
                s.Email,
                s.Points,
                Coupons = couponList,
            }).FirstOrDefault(x => x.Id == id);
            if (query == null) return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูลผู้ใช้งาน", Data = { } };
            return new ResponseData { StatusCode = 200, TaskStatus = true, Data = query, Message = "Sucess" };
        }

        public async Task<ResponseMessage> Register(RegsiterReq data)
        {
            var chk = _dataContext.Users.AsNoTracking().FirstOrDefault(x => x.Email == data.Email || x.Phone == data.Phone);
            if (chk != null)
            {
                return new ResponseMessage { Message = "อีเมลหรือเบอร์โทรศัพท์นี้ เคยลงทะเบียนแล้ว", StatusCode = 200, TaskStatus = false };
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);

            if (data.Upload != null)
            {

                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Profile");
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
                data.ImageProfile = uniqueFileName;
            }

            data.Password = hashedPassword;
            data.Create_At = DateTime.Now;
            data.Role = 1;
            data.Points = 0;
            data.Id = generateCode();

            await _dataContext.AddAsync(data);
            int save = await _dataContext.SaveChangesAsync();
            if (save > 0)
            {
                return new ResponseMessage { Message = "ลงทะเบียนสำเร็จ", StatusCode = 200, TaskStatus = true };
            }
            return new ResponseMessage { Message = "ลงทะเบียนไม่สำเร็จ", StatusCode = 200, TaskStatus = false };
        }

        public async Task<ResponseData> Login(LoginReq data)
        {
            var chkAccount = await _dataContext.Users.FirstOrDefaultAsync(x => x.Email == data.EmailOrPhone || x.Phone == data.EmailOrPhone);
            if (chkAccount == null) return new ResponseData() { StatusCode = 200, Message = "ไม่พบบัญชีผู้ใช้", TaskStatus = false, Data = { } };

            bool isMatch = BCrypt.Net.BCrypt.Verify(data.Password, chkAccount.Password);
            if (!isMatch)
            {
                return new ResponseData() { StatusCode = 200, TaskStatus = false, Message = "รหัสผ่านไม่ถูกต้อง", Data = { } };
            }
            else
            {
                var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Profile/";
                string token = _token.GenerateToken(chkAccount.Email, chkAccount.Role.ToString());
                var couponList = await _dataContext.CouponUsers.Where(x => x.UsersId == chkAccount.Id && x.IsUsed == 0)
                    .Select(s => s.CouponsId).ToListAsync();
                var value = new
                {
                    Id = chkAccount.Id,
                    Email = chkAccount.Email,
                    prefix = chkAccount.Prefix,
                    firstname = chkAccount.FirstName,
                    lastname = chkAccount.LastName,
                    imageProfile = url+chkAccount.ImageProfile,
                    phone = chkAccount.Phone,
                    token = token,
                    Points = chkAccount.Points,
                    Role = chkAccount.Role,
                    Coupons = couponList,
                    isLogin = true,
                };
                return new ResponseData() { StatusCode = 200, TaskStatus = true, Message = "Sucess", Data = value };
            }
        }

        public async Task<ResponseData> UpdateUser(UpdateUserReq data )
        {
            var checkEmail = await _dataContext.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Email == data.Email && a.Id != data.Id);
            if (checkEmail != null) return new ResponseData() { StatusCode = 200, TaskStatus = false, Message = string.Format("อีเมล {0} เคยใช้งานแล้ว", data.Email) };

            var user = _dataContext.Users.AsNoTracking().FirstOrDefault(a => a.Id == data.Id);
            if (user != null)
            {
                string image = "";
                var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Profile/";
                if (data.Upload != null)
                {
                    string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Profile");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    if (!_checkFile.IsImageFile(data.Upload))
                    {
                        return new ResponseData { Message = "กรุณาอัพโหลดไฟล์รูปภาพ เท่านั้น!!", StatusCode = 200, TaskStatus = false , Data = { } };
                    }

                    string typefile = Path.GetExtension(data.Upload.FileName);
                    image = Guid.NewGuid().ToString() + typefile;
                    string filePath = Path.Combine(uploadsFolder, image);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await data.Upload.CopyToAsync(fileStream);
                    }
                }
                user.Prefix = data.Prefix;
                user.FirstName = data.FirstName;
                user.LastName = data.LastName;
                user.Email = data.Email;
                user.Phone = data.Phone;
                user.ImageProfile = !string.IsNullOrEmpty(image) ? image : user.ImageProfile;
                _dataContext.Update(user);
                int save = await _dataContext.SaveChangesAsync();

                if (save > 0)
                {
                    var value = new
                    {
                        Id = user.Id,
                        Email = user.Email,
                        prefix = user.Prefix,
                        firstname = user.FirstName,
                        lastname = user.LastName,
                        imageProfile = url + user.ImageProfile,
                        phone = user.Phone,
                        isLogin = true,
                    };
                    return new ResponseData() { StatusCode = 200, TaskStatus = true, Message = "Sucess", Data = value };
                }
                return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "Unsuccess" };
            }
            return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "Unsuccess" };
        }

        public async Task<ResponseData> GetCouponUserList(string userId)
        {
            var query = _dataContext.CouponUsers.Include(x=>x.Coupons).Where(x => x.UsersId == userId && x.IsUsed == 0)
                .Select(s=> new
                {
                    s.Id,
                    CouponId = s.Coupons.Id,
                    CouponName = s.Coupons.Name,
                    CouponDiscount =  s.Coupons.Discount,
                    ExpiredAt = s.Coupons.ExpiredAt.Value.Date.ToString("dd/MM/yyyy")
                }).ToList();
            return new ResponseData { StatusCode = 200, Message = "Success", TaskStatus = true, Data = query };
        }

        public async Task<ResponseMessage> RedeemCoupon(string userId, int couponId)
        {
            var coupon = await _dataContext.Coupons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == couponId);
            if (coupon != null)
            {
                var user = await _dataContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
                if (user != null)
                {
                    if (user.Points >= coupon.Points && coupon.limit != 0)
                    {
                        user.Points = user.Points - coupon.Points;
                        coupon.limit -= 1;
                        _dataContext.Update(user);
                        _dataContext.Update(coupon);
                        await _dataContext.CouponUsers.AddAsync(new CouponUser
                        {
                            UsersId = userId,
                            CouponsId = couponId,
                            IsUsed = 0,
                            CreatedAt = DateTime.Now
                        });
                        int save =  await _dataContext.SaveChangesAsync();
                        if(save == 3)
                        {
                            return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                        }
                    }
                }
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }
        
        public async Task<ResponseData> GetHistoryOrderList(string userId)
        {
            var query = await _dataContext.Orders.Include(o => o.OrderDetail).Where(x=>x.UsersId == userId).Select(s=> new
            {
                s.Id,
                s.OrderDate,
                s.OrderStatus,
                s.TotalPrice,
                CountDetail = s.OrderDetail.Count,
                s.IsReview
            }).OrderByDescending(x=>x.OrderDate).ToListAsync();

            return new ResponseData {StatusCode = 200 ,TaskStatus = true ,Message = "Success",Data = query};
        }

        public async Task<ResponseData> GetHistoryCoupon (string userId)
        {
            var query = await _dataContext.CouponUsers.Where(x=>x.UsersId == userId).Include(x => x.Coupons).Select(s => new
            {
                Name =  s.Coupons.Name,
                Points = s.Coupons.Points,
                CreatedAt = s.CreatedAt,
                Discount = s.Coupons.Discount,
            }).ToListAsync();
            return new ResponseData { StatusCode = 200 ,TaskStatus = true , Message = "Success" , Data = query};
        }

        private string generateCode()
        {
            string lastYear = DateTime.Today.ToString("yy", new CultureInfo("th-TH"));
            var Auto = _dataContext.Users.Where(x => x.Role == 1).OrderByDescending(z => z.Id).FirstOrDefault();
            if (Auto == null)
            {
                return lastYear + "000001";
            }
            else
            {
                var g = Auto.Id.Length;
                var length = g - 6;
                var lastz = Auto.Id.Substring(6, length);
                return lastYear + (Convert.ToInt32(lastz) + 1).ToString("D6");
            }
        }
    }
}
