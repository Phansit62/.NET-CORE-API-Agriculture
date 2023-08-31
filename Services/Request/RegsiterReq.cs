using System.ComponentModel.DataAnnotations;

namespace Agriculture.Services.Request
{
    public class RegsiterReq:Users
    {
        public IFormFile? Upload { get; set; }
    }
}
