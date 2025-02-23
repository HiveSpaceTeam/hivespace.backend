using HiveSpace.Domain.SeedWork;

namespace HiveSpace.Domain.AggergateModels;

public class Category : AggregateRoot<int>
{
    public string Name { get; private set; }

    public string DisplayName { get; private set; }

    public string? FileImageId { get; private set; }
 
    private readonly List<Category> _children = [];

    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    public bool HasChildren => _children.Count > 0;

    public int? ParentId { get; private set; }
    public Category? Parent { get; private set; }

    private readonly List<AttributeProduct> _attributes = [];
    public IReadOnlyCollection<AttributeProduct> Attributes => _attributes.AsReadOnly();

    public Category(int id, string name, string displayName, int? parentId, string? fileImageId = null)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        ParentId = parentId;
        FileImageId = fileImageId;
    }
}
