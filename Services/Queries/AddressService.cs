using Agriculture.Services.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Services.Response;

namespace Agriculture.Services.Queries
{
    public class AddressService
    {
        private readonly DataContext _dataContext;

        public AddressService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ResponseData> GetAddress(string id)
        {
            var query = await _dataContext.Addresses.Include(i=>i.User)
                                                    .Where(x=> x.User.Id == id).Select(s=> new
                                                    {
                                                        s.Id ,
                                                        s.Name ,
                                                        s.AddressAt,
                                                        s.Is_default,
                                                        s.District,
                                                        s.Subdistrict,
                                                        s.Postcode,
                                                        s.Province,
                                                    }).OrderByDescending(o=>o.Is_default).ToListAsync();
            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        public async Task<ResponseMessage> CreateAddress(AddressReq data)
        {
            var value  = await  _dataContext.Addresses.AddAsync(new Address
            {
                Name= data.Name ,
                AddressAt= data.AddressAt ,
                District=data.District ,
                Is_default = data.Is_default,
                Postcode = data.Postcode ,
                Province = data.Province ,
                Subdistrict = data.Subdistrict ,
                UserId = data.UserId ,
            });
            int save = await _dataContext.SaveChangesAsync();
            if(save > 0)
            {
                if(data.Is_default == 1)
                {
                    var chk  = _dataContext.Addresses.Where(x=>x.UserId == data.UserId && x.Is_default == 1 && x.Id != value.Entity.Id).ToList();
                    if(chk.Count > 0)
                    {
                        foreach(var item in chk)
                        {
                            item.Is_default = 0;
                            _dataContext.Addresses.UpdateRange(item);
                        }
                        int update = await _dataContext.SaveChangesAsync();
                        if(update == chk.Count)
                        {
                            return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                        }
                    }
                }
                return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };

            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };

        }
        public async Task<ResponseData> GetAddressDetail(int id)
        {
            var query = await _dataContext.Addresses.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id);
            if(query == null) return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" , Data = { } };

            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success" , Data = query };
        }

        public async Task<ResponseMessage> UpdateAddress(AddressReq data)
        {
            _dataContext.Update(new Address
            {
                Id = (int)data.Id,
                Name = data.Name,
                AddressAt = data.AddressAt,
                District = data.District,
                Is_default = data.Is_default,
                Postcode = data.Postcode,
                Province = data.Province,
                Subdistrict = data.Subdistrict,
                UserId = data.UserId,
            });
            int save = await _dataContext.SaveChangesAsync();
            if (save > 0)
            {
                if (data.Is_default == 1)
                {
                    var chk = _dataContext.Addresses.Where(x => x.UserId == data.UserId && x.Is_default == 1 && x.Id != data.Id).ToList();
                    if (chk.Count > 0)
                    {
                        foreach (var item in chk)
                        {
                            item.Is_default = 0;
                            _dataContext.Addresses.UpdateRange(item);
                        }
                        int update = await _dataContext.SaveChangesAsync();
                        if (update == chk.Count)
                        {
                            return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                        }
                    }
                }
                return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };

        }

        public async Task<ResponseMessage> DeleteAddress(int id)
        {
            var query = await _dataContext.Addresses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (query != null)
            {
                _dataContext.Remove(query);
                int save = await _dataContext.SaveChangesAsync();
                if (save > 0)
                {
                    return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                }
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "UnSuccess" };
        }
    }
}
