using HiveSpace.Domain.SeedWork;

namespace HiveSpace.Domain.AggergateModels.ProductAggregate;
public class ProductVariantOption : ValueObject
{
    public string Value { get; private set; }

    private ProductVariantOption()
    {
    }

    public ProductVariantOption(string value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
