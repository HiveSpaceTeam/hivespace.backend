namespace HiveSpace.Application.Models.Dtos.Request.CartItem
{
    public class UpdateCartItemRequestDto
    {
        public int Id { get; set; }
        public Guid CartId { get; set; }
        public int Quantity { get; set; }

        public int? OldQuantity { get; set; }
    }
}
