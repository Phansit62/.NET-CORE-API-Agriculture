namespace PRAWAN_EV_API.Services.Response
{
    public class ResponseMessage
    {
        public int StatusCode { get; set; }
        public bool TaskStatus { get; set; } = false;
        public string Message { get; set; }
    }
}
