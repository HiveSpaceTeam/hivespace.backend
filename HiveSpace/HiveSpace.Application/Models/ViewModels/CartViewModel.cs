namespace HiveSpace.Application.Models.ViewModels
{
    public class CartViewModel
    {
        public Guid Id { get; set; }
        public List<CartItemViewModel> Items { get; set; }
    }

    public class CartItemViewModel
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductVariantName { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
        public string Currency { get; set; }
        public int SkuId { get; set; }
        public bool IsSelected { get; set; }
    }
}
