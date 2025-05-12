using HiveSpace.Application.Models.Dtos.Request.Paging;

namespace HiveSpace.Application.Models.Dtos.Request.Product
{
    public class ProductSearchRequestDto : PagingRequestDto
    {
        public string Keyword { get; set; } = "";
        public string Sort { get; set; } = "ASC";
    }
}
