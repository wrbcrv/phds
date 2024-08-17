namespace Api.DTOs
{
    public class PagedResponseDTO<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Total { get; set; }
    }
}
