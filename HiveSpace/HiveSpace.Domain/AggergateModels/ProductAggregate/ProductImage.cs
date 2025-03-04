using HiveSpace.Domain.SeedWork;

namespace HiveSpace.Domain.AggergateModels.ProductAggregate;
public class ProductImage(string productId, string fileId) : ValueObject
{
    public string ProductId { get; private set; } = productId;
    public string FileId { get; private set; } = fileId;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
        yield return FileId;
    }
}
