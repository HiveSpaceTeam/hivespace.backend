using Newtonsoft.Json;

namespace HiveSpace.TestDataLoader.Features.Models;

public class CategoryShoppe
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("parent_id")]
    public int ParentId { get; set; }

    [JsonProperty("has_active_children")]
    public bool HasActiveChildren { get; set; }

    [JsonProperty("has_children")]
    public bool HasChildren { get; set; }

    [JsonProperty("children")]
    public List<CategoryShoppe> Children { get; set; } = new List<CategoryShoppe>();

    [JsonProperty("region_setting")]
    public object RegionSetting { get; set; } = new();

    [JsonProperty("is_prohibit")]
    public bool IsProhibit { get; set; }
}