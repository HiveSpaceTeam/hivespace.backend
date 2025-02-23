namespace HiveSpace.Application.Models.Dtos.Request.ShoppingCart
{
    public class UpdateMultiCartItemSelectionDto
    {
        public List<int> SkuIds { get; set; }
        public Guid CartId { get; set; }
        public bool IsSelected { get; set; }
    }
}
