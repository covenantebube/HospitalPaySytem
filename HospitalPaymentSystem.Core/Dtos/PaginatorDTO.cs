﻿
namespace HospitalPaymentSystem.Core.Dtos
{

    public class PaginatorDTO<T>
    {
        public T? PageItems { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int NumberOfPages { get; set; }
    }
}
