namespace HospitalPaymentSystem.Core.Utilities
{
    public class PaginationFilter
    {
        public PaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize is > 10 or < 1 ? 10 : pageSize;
        }

        public PaginationFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public int PageNumber { get; }
        public int PageSize { get; }
    }

}
