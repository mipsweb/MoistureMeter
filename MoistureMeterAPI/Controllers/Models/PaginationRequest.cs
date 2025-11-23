namespace MoistureMeterAPI.Controllers.Models
{
    public class PaginationRequest
    {
        public required int PageSize { get; set; }
        public string? LastId { get; set; }
    }
}
