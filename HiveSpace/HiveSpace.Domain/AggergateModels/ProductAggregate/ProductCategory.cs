using HiveSpace.Domain.SeedWork;

namespace HiveSpace.Domain.AggergateModels.ProductAggregate;
public class ProductCategory(int productId, int categoryId) : ValueObject
{
    public int ProductId { get; private set; } = productId;

    public int CategoryId { get; private set; } = categoryId;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return CategoryId;
    }
}
