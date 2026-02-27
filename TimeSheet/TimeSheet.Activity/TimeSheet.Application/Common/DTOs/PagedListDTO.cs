using System;
using System.Collections.Generic;

namespace TimeSheet.Application.Common.DTOs
{
    public class PagedListDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; } = null;
        public string? FirstLetter { get; set; } = null;
        public string Order { get; set; } = "asc";

        public PagedListDTO() { }

        public PagedListDTO(int pageNumber, int pageSize, string searchTerm, string firstLetter, string order)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchTerm = searchTerm;
            FirstLetter = firstLetter;
            Order = order;
        }
    }
}