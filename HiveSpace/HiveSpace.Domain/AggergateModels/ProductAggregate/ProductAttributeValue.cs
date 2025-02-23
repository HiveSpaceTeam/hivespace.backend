using HiveSpace.Domain.Seedwork;

namespace HiveSpace.Domain.AggergateModels.ProductAggregate;
public class ProductAttributeValue : Entity<int>
{
    public int AttributeId { get; private set; }

    public int? ValueId { get; private set; }

    public string? RawValue { get; private set; }

    public ProductAttributeValue(int attributeId, int? valueId, string? rawValue)
    {
        
        AttributeId = attributeId;
        ValueId = valueId;
        RawValue = rawValue;
        if (IsInvalid())
        {
            throw new Exception("Invalid attribute value");
        }
    }

    private bool IsInvalid()
    {
        return (string.IsNullOrEmpty(RawValue) && (ValueId == 0 || ValueId is null)) || (!string.IsNullOrEmpty(RawValue) && ValueId is not null && ValueId != 0) 
            || AttributeId == 0;
    }

    public void UpdateRawValue(string rawValue)
    {
        RawValue = rawValue;
        ValueId = null;
    }

    public void UpdateValueId(int valueId)
    {
        ValueId = valueId;
        RawValue = null;
    }
}
