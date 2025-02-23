using HiveSpace.Domain.Seedwork;

namespace HiveSpace.Domain.AggergateModels.ShoppingCartAggregate;
public class CartItem : Entity<Guid>
{
    public int Quantity { get; private set; }

    public int SkuId { get; private set; }

    public bool IsSelected { get; private set; }

    public CartItem(int skuId, int quantity)
    {
        SkuId = skuId;
        Quantity = quantity;
        if (IsInvalid())
        {
            throw new Exception("Invalid cart item");
        }
    }

    public void SetQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new Exception("Invalid cart item");
        }
        Quantity = quantity;
    }

    public void SetIsSelected(bool isSelected)
    {
        IsSelected = isSelected;
    }

    private bool IsInvalid()
    {
        return Quantity <= 0 || SkuId <= 0;
    }
}
