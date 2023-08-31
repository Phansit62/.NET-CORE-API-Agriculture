using Agriculture.Services.Request;
using Microsoft.EntityFrameworkCore;
using PRAWAN_EV_API.Services.Response;

namespace Agriculture.Services.Queries
{
    public class TransferPointService
    {
        private readonly DataContext _dataContext;

        public TransferPointService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ResponseData> GetTransferPointList(string userId)
        {
            var chkUser = await _dataContext.Users.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == userId);
            if(chkUser == null)
            {
                return new ResponseData { StatusCode = 200, TaskStatus = false, Message = "ไม่พบรหัสผุู้ใช้", Data = { } };
            }
            var query = await _dataContext.TransferPoints.Where(x=>x.UsersId == userId).Include(i=> i.Users).Include(i=>i.ToUsers).Select(x=> new
            {
                NameUser = x.Users.FirstName,
                NameToUser = x.ToUsers.FirstName,
                x.points,
                x.CreatedAt
            }).ToListAsync();

            return new ResponseData { StatusCode = 200, TaskStatus = true, Message = "Success", Data = query };
        }

        public async Task<ResponseMessage> Create (TransferPointReq data)
        {
            var chk = await _dataContext.Users.FirstOrDefaultAsync(x => x.Phone == data.Telephone);
            if( chk != null )
            {
                await _dataContext.TransferPoints.AddAsync(new TransferPoint
                {
                    ToUsersId = chk.Id,
                    UsersId = data.UserId,
                    points = data.Points,
                    CreatedAt = DateTime.Now,
                });
                int save = await _dataContext.SaveChangesAsync();
                if (save > 0)
                {
                    List<Users> users = new List<Users>();
                    chk.Points += data.Points;
                    users.Add(chk);

                    var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == data.UserId);
                    user.Points -= data.Points;
                    users.Add(user);

                    _dataContext.UpdateRange(users);
                    await _dataContext.SaveChangesAsync();

                    return new ResponseMessage { StatusCode = 200, TaskStatus = true, Message = "Success" };
                }
            }
            else
            {
                return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "ไม่พบข้อมูลผู้ใช้งาน" };
            }
            return new ResponseMessage { StatusCode = 200, TaskStatus = false, Message = "Unsuccess" };
        }
    }
}
