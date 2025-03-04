using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.SeedWork;

namespace HiveSpace.Domain.AggergateModels.ShoppingCartAggregate;

public class ShoppingCart(Guid customerId) : AggregateRoot<Guid>
{
    public Guid CustomerId { get; private set; } = customerId;

    private readonly List<CartItem> _items = [];
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public CartItem AddItem(int quantity, int skuId, bool isSelected)
    {
        CartItem? item = _items.FirstOrDefault(x => x.SkuId == skuId);

        if (item is null)
        {
            item = new CartItem(skuId, quantity);
            _items.Add(item);
        }
        else
        {
            item.SetQuantity(quantity + item.Quantity);
        }
        item.SetIsSelected(isSelected);
        return item;
    }

    public void RemoveItem(Guid cartItem)
    {
        var deletedItemIndex = _items.FindIndex(x => x.Id == cartItem);
        if (deletedItemIndex == -1) throw new NotFoundException("Item not found");
        _items.RemoveAt(deletedItemIndex);
    }

    public void RemoveItems(List<Guid> cartItems)
    {
        _items.RemoveAll(item => cartItems.Contains(item.Id));
    }

    public void UpdateCartItem(CartItem cartItem)
    {
        var foundCartItem = _items.Find(x => x.SkuId == cartItem.SkuId);
        if (foundCartItem is null) throw new NotFoundException("Item not found");

        foundCartItem.SetQuantity(cartItem.Quantity);
    }



    public void UpdateSelectionCartItem(List<int> skuIds, bool isSelected)
    {
        _items.ForEach(item =>
        {
            if (skuIds.Contains(item.SkuId))
            {
                item.SetIsSelected(isSelected);
            }
        });
    }

}