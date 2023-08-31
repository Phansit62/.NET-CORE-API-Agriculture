using Agriculture.Services.Request;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Helpers;
using PRAWAN_EV_API.Services.Response;

namespace Agriculture.Services.Queries
{
    public class NewsService
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CheckFile _checkFile;
        public NewsService(DataContext context, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor, CheckFile checkFile)
        {
            _context = context;
            _checkFile = checkFile;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponsePagination> GetNews(int pagesize ,int currentpage , string search)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/News/";
            var query = await _context.News
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                    s.Description,
                    Image = $"{url}{s.Imagebanner}"
                }).Where(x => !string.IsNullOrEmpty(search) ? x.Title.Contains(search) : true).ToListAsync();
            Pagination pagination = new Pagination(query.ToList<dynamic>(), currentpage, pagesize);
            return new ResponsePagination { StatusCode = 200, TaskStatus = true, Message = "สำเร็จ", Pagin = new { Currentpage = currentpage, Pagesize = pagesize, Totalrows = pagination.TotalRow, Totalpages = pagination.TotalPage }, Data = pagination.Values };
        }

        public async Task<ResponseData> GetNewsList()
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/News/";
            var query = await _context.News
               .Select(s => new
               {
                   Image = $"{url}{s.Imagebanner}"
               }).ToListAsync();
            return new ResponseData { StatusCode = 200 ,TaskStatus = true ,Message = "Success" , Data = query };
        }

        public async Task<ResponseData> GetNewsDetail (int id)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/News/";
            var query = await _context.News.Select(s=> new
            {
                s.Id,
                s.Title,
                s.Description,
                Image = $"{url}{s.Imagebanner}"
            }).FirstOrDefaultAsync(x => x.Id == id);
            if (query == null) return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "UnSuccess", Data = { } };
            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        public async Task<ResponseMessage> Create (NewsReq data)
        {
            if(data.Upload != null)
            {
                if (!_checkFile.IsImageFile(data.Upload))
                {
                    return new ResponseMessage { Message = "กรุณาอัพโหลดไฟล์รูปภาพ เท่านั้น!!", StatusCode = 200, TaskStatus = false };
                }

                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "News");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                string typefile = Path.GetExtension(data.Upload.FileName);
                var uniqueFileName = Guid.NewGuid().ToString() + typefile;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await data.Upload.CopyToAsync(stream);
                }

                await _context.News.AddAsync(new News
                {
                    Title = data.Title,
                    Description = data.Description,
                    Imagebanner = uniqueFileName
                });

                int save = await _context.SaveChangesAsync();
                if (save > 0)
                {
                    return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                }
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }

        public async Task<ResponseMessage> Update(NewsReq data)
        {
            var chk = await _context.News.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.Id);
            if(chk != null)
            {
                if(data.Upload != null)
                {
                    if (!_checkFile.IsImageFile(data.Upload))
                    {
                        return new ResponseMessage { Message = "กรุณาอัพโหลดไฟล์รูปภาพ เท่านั้น!!", StatusCode = 200, TaskStatus = false };
                    }

                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "News");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    string typefile = Path.GetExtension(data.Upload.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + typefile;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await data.Upload.CopyToAsync(stream);
                    }

                    var newData = new News
                    {
                        Id = (int)data.Id,
                        Title = data.Title,
                        Description = data.Description,
                        Imagebanner = uniqueFileName
                    };

                    _context.Entry(newData).State = EntityState.Modified;
                    int save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                    }
                }
                return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูล" };
        }

        public async Task<ResponseMessage> Delete(int id)
        {
            var chk = await _context.News.FirstOrDefaultAsync(x => x.Id == id);
            if(chk != null)
            {
                _context.Remove(chk);
                int save = await _context.SaveChangesAsync();
                if(save > 0)
                {
                    return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                }
                return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูล" };
        }
    }
}
