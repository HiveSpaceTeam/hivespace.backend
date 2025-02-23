namespace HiveSpace.Application.Models.Dtos.Request.ShoppingCart
{
    public class AddItemToCartRequestDto
    {
        public int SkuId { get; set; }
        public int Quantity { get; set; }
        public bool IsSelected { get; set; }
    }
}
