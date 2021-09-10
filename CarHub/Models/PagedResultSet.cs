using System.Collections.Generic;

namespace CarHub.Models
{
    public class PagedResultSet<T> where T : class
    {
        public int TotalCount { get; init; }
        public IEnumerable<T> Items { get; init; }
        public int CurrentPage { get; init; }
    }
}
