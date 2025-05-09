using HiveSpace.Domain.Shared;

namespace HiveSpace.Application.Models.Dtos.Request.Paging
{
    public class PagingRequestDto
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public Dictionary<string, FilterItem>? Filters { get; set; }

        // sort, filter
    }
}
