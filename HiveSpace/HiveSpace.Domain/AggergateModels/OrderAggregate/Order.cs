using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.SeedWork;

namespace HiveSpace.Domain.AggergateModels.OrderAggregate;
public class Order : AggregateRoot<Guid>
{
    public Guid CustomerId { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public double SubTotal => Items.Sum(x => x.Price.Amount);

    public double ShippingFee { get; private set; }

    public double Discount { get; private set; }

    public double TotalPrice => SubTotal + ShippingFee - Discount;

    public DateTimeOffset OrderDate { get; private set; }

    public ShippingAddress ShippingAddress { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.PendingApproval;

    public PaymentMethod PaymentMethod { get; private set; }

    private Order()
    {
    }

    public Order(Guid customerId, double shippingFee, double discount, DateTimeOffset orderDate, ShippingAddressProps shippingAddressProps, PaymentMethod paymentMethod, List<OrderItemProps> orderItemProps)
    {
        CustomerId = customerId;
        ShippingFee = shippingFee;
        Discount = discount;
        OrderDate = orderDate;
        ShippingAddress = new ShippingAddress(
            shippingAddressProps.FullName,
            shippingAddressProps.PhoneNumber,
            shippingAddressProps.OtherDetails,
            shippingAddressProps.Street,
            shippingAddressProps.Ward,
            shippingAddressProps.District,
            shippingAddressProps.Province,
            shippingAddressProps.Country
            );
        PaymentMethod = paymentMethod;
        orderItemProps.ForEach(x => AddItem(x.Quantity, x.SkuId, x.ProductName, x.VariantName, x.Thumbnail, x.Amount, x.Currency));
        if (IsInvalid()) throw new DomainException
        {
            MessageCode = "i18nOrder.messages.orderInvalid"
        };
    }

    private bool IsInvalid()
    {
        return Items.Count == 0;
    }

    public void AddItem(int quantity, int skuId, string productName, string variantName, string thumNail, double amount, Currency currency)
    {
        var item = new OrderItem(skuId, productName, variantName, thumNail, quantity, amount, currency);
        _items.Add(item);
    }

    public void RemoveItem(int skuId)
    {
        var deletedItemIndex = _items.FindIndex(x => x.SkuId == skuId);
        if (deletedItemIndex == -1) throw new Exception("Item not found");
        _items.RemoveAt(deletedItemIndex);
    }
}
