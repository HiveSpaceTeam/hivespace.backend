using HiveSpace.Domain.SeedWork;

namespace HiveSpace.Domain.AggergateModels;
public class AttributeProduct(string name, string displayName, int? parentId, bool mandatory, int valueId) : AggregateRoot<int>
{
    public string Name { get; private set; } = name;

    public int ValueId { get; private set; } = valueId;

    public string DisplayName { get; private set; } = displayName;

    public int? ParentId { get; private set; } = parentId;
    public AttributeProduct? Parent { get; private set; }

    private readonly List<AttributeProduct> _children = [];
    public IReadOnlyCollection<AttributeProduct> Children => _children.AsReadOnly();

    private readonly List<Category> _categories = [];
    public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();

    public bool Mandatory { get; private set; } = mandatory;

    public void AddCategory(Category category)
    {
        if (_categories.All(c => c.Id != category.Id))
        {
            _categories.Add(category);
        }
    }


}
