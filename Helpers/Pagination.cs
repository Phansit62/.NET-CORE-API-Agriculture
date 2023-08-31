namespace PRAWAN_EV_API.Helpers
{
    public class Pagination
    {
        public int TotalRow { get; set; }
        public int TotalPage { get; set; }
        public List<object> Values { get; set; }

        public Pagination(List<object> query, int currentPage, int pageSize)
        {
            TotalRow = query.Count();
            TotalPage = (int)Math.Ceiling((double)TotalRow / pageSize);
            Values = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
