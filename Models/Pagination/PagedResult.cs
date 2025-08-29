// Models/Pagination/PagedResult.cs
namespace painel_conversas.Models.Pagination
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartItem => TotalItems == 0 ? 0 : (CurrentPage - 1) * PageSize + 1;
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);
        
        public static PagedResult<T> Create(List<T> source, int page, int pageSize)
        {
            var totalItems = source.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            return new PagedResult<T>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}

// Models/Pagination/PaginationViewModel.cs
namespace painel_conversas.Models.Pagination
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int TotalItems { get; set; }
        public int StartItem { get; set; }
        public int EndItem { get; set; }
        public int PageSize { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public object RouteValues { get; set; }
    }
}