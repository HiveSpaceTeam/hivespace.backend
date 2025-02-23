using Newtonsoft.Json;

namespace HiveSpace.TestDataLoader.Features.Models;
public class CategoryJson
{
    [JsonProperty("catid")]
    public int Catid { get; set; }

    [JsonProperty("parent_catid")]
    public int ParentCatid { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("image")]
    public string Image { get; set; } = string.Empty;

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("children")]
    public List<CategoryJson>? Children { get; set; }
}
