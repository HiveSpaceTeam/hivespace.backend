using HiveSpace.Domain.Seedwork;

namespace HiveSpace.Domain.AggergateModels.ProductAggregate;
public class ProductVariant : Entity<int>
{
    public string Name { get; private set; }

    private readonly List<ProductVariantOption> _options = [];
    public IReadOnlyCollection<ProductVariantOption> Options => _options.AsReadOnly();

    private ProductVariant()
    {
    }

    public ProductVariant(string name, List<ProductVariantOption> options)
    {
        Name = name;
        _options = options;

        if (IsInvalid())
        {
            throw new Exception("Invalid product variant");
        }
    }

    private bool IsInvalid()
    {
        return _options is null || _options.Count <= 1;
    }
}
