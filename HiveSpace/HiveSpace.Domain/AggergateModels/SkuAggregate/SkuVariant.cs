using HiveSpace.Domain.SeedWork;
using System.Text.Json.Serialization;

namespace HiveSpace.Domain.AggergateModels.SkuAggregate;

public class SkuVariant : ValueObject
{
    public string Name { get; private set; }
    public string Value { get; private set; }

    [JsonConstructor]
    public SkuVariant(string name, string value)
    {
        Name = name;
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Value;
    }
}
