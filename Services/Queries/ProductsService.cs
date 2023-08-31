using Agriculture.Helpers;
using Agriculture.Models;
using Agriculture.Services.Request;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Helpers;
using PRAWAN_EV_API.Services.Response;
using System;

namespace Agriculture.Services.Queries
{
    public class ProductsService
    {
        private readonly DataContext _dataContext;
        private readonly CheckFile _checkFile;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsService(DataContext dataContext, CheckFile checkFile, IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor) 
        {
            _dataContext = dataContext;
            _checkFile = checkFile;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponsePagination> GetProducts(int pagesize, int currentpage, string search)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/ProductImage/";
            var query = await _dataContext.Products.Include(i=>i.Images).Select(s=> new
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price,
                Image = s.Images.Count > 0 ? url+s.Images[0].Path : "",
                category = s.Category,
                s.Description,
                s.Stock,
                s.Unit
            }).Where(x=> !string.IsNullOrEmpty(search) ? x.Name.Contains(search) : true)
              .ToListAsync<dynamic>();

            Pagination pagination = new Pagination(query, currentpage, pagesize);

            return new ResponsePagination { StatusCode = 200, TaskStatus = true, Message = "สำเร็จ", Pagin = new { Currentpage = currentpage, Pagesize = pagesize, Totalrows = pagination.TotalRow, Totalpages = pagination.TotalPage }, Data = pagination.Values };
        }

        public async Task<ResponseData> GetProductDetail(int productId)
        {
            var url = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/ProductImage/";
            var product = await _dataContext.Products.Include(x => x.Images).Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                Price = s.Price,
                Image = s.Images.Select(img=> new
                {
                    Id = img.Id,
                    Path = url + img.Path,
                }).ToList(),
                category = s.Category,
                s.Description,
                s.Stock,
                s.Unit
            }).FirstOrDefaultAsync(x => x.Id == productId);


            if (product == null)
            {
                return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูลสินค้า" ,Data = { } };
            }

            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = product };
        }

        public async Task<ResponseMessage> CreateProduct(ProductReq data)
        {
            var onCreate = await _dataContext.AddAsync(new Products
            {
                Name = data.Name,
                Category = data.Category,
                Description = data.Description,
                Price = data.Price,
                Unit = data.Unit,
                Stock = data.Stock,
            });
            int save = await _dataContext.SaveChangesAsync();
            if (save > 0)
            {
                List<Images> images = new List<Images>();

                if (data.Upload != null && data.Upload.Count() > 0)
                {
                    foreach (var file in data.Upload)
                    {
                        if (file.Length > 0)
                        {
                            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "ProductImage");
                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);
                            string typefile = Path.GetExtension(file.FileName);
                            var uniqueFileName = Guid.NewGuid().ToString() + typefile;
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            images.Add(new Images
                            {
                                Path = uniqueFileName,
                                ProductsId = onCreate.Entity.Id
                            });
                        }
                    }

                    _dataContext.AddRange(images);
                    int saveImage = await _dataContext.SaveChangesAsync();
                    if (saveImage > 0)
                    {
                        return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                    }
                }
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }

        public async Task<ResponseMessage> UpdateProduct( ProductReq data)
        {
            var product = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == data.Id);

            if (product == null)
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูลสินค้า" };
            }

            if(data.removeImage != null && data.removeImage.Length > 0)
            {
                var existingImages = await _dataContext.Images.Where(img => data.removeImage.Contains(img.Id)).ToListAsync();
                _dataContext.Images.RemoveRange(existingImages);
            }
            // Update the product data
            product.Name = data.Name;
            product.Category = data.Category;
            product.Description = data.Description;
            product.Price = data.Price;
            product.Unit = data.Unit;
            product.Stock = data.Stock;

            _dataContext.Products.Update(product);
            int save = await _dataContext.SaveChangesAsync();

            if (save > 0)
            {
               
                // Add new images
                List<Images> images = new List<Images>();

                if (data.Upload != null && data.Upload.Count() > 0)
                {
                    foreach (var file in data.Upload)
                    {
                        if (file.Length > 0)
                        {
                            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "ProductImage");
                            string typefile = Path.GetExtension(file.FileName);
                            var uniqueFileName = Guid.NewGuid().ToString() + typefile;
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            images.Add(new Images
                            {
                                Path = uniqueFileName,
                                ProductsId =data.Id
                            });
                        }
                    }
                }
                _dataContext.AddRange(images);
                int saveImages = await _dataContext.SaveChangesAsync();
                return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Product updated successfully" };
            }

            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "Failed to update product" };
        }

        public async Task<ResponseMessage> DeleteProduct(int productId)
        {
            var product = await _dataContext.Products.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูลสินค้า" };
            }

            foreach (var image in product.Images)
            {
                var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "ProductImage", image.Path);
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }

            _dataContext.Images.RemoveRange(product.Images);
            _dataContext.Products.Remove(product);

            int save = await _dataContext.SaveChangesAsync();

            if (save > 0)
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Product deleted successfully" };
            }

            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "Failed to delete product" };
        }

    }
}
