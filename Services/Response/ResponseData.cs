namespace PRAWAN_EV_API.Services.Response
{
    public class ResponseData
    {
        public int StatusCode { get; set; }
        public bool TaskStatus { get; set; } = false;
        public string Message { get; set; }
        public object Data { get; set; } = new object();
    }
}
