using System;
using System.Collections.Generic;

namespace TimeSheet.Domain.Common.Models
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? FirstLetter { get; set; }

        public PagedList() { }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize, string? firstLetter)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
            FirstLetter = firstLetter;
        }
    }
}