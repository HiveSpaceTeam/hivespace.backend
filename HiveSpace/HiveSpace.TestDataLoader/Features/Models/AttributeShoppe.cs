using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HiveSpace.TestDataLoader.Features.Models;
public class AttributeShoppe
{
    [JsonPropertyName("attribute_id")]
    public int AttributeId { get; set; }

    [JsonPropertyName("value_id")]
    public int ValueId { get; set; }

    [JsonPropertyName("parent_attribute_id")]
    public int ParentAttributeId { get; set; }

    [JsonPropertyName("parent_value_id")]
    public int ParentValueId { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("mandatory")]
    public bool Mandatory { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("value_type")]
    public int ValueType { get; set; }

    [JsonPropertyName("children")]
    public List<AttributeShoppe> Children { get; set; } = [];

    [JsonPropertyName("attribute_info")]
    public object AttributeInfo { get; set; } = new();

    [JsonPropertyName("attribute_model_id")]
    public int AttributeModelId { get; set; }

    [JsonPropertyName("multi_lang")]
    public List<object> MultiLang { get; set; } = [];
}

public class AttributeTreeResponse
{
    [JsonPropertyName("attribute_tree")]
    public List<AttributeShoppe> AttributeTree { get; set; } = [];
}

public class AttributeTreeRequest
{

    [JsonProperty("category_id")]
    public int CategoryId { get; set; }

    [JsonProperty("locale")]
    public string Locale { get; set; } = string.Empty;
}