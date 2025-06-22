using System.Text.Json.Serialization;

namespace MiracleListVueHybridDemo.Dtos
{
  public class Category
  {
    [JsonPropertyName("categoryID")]
    public int CategoryID { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("tasks")]
    public MiracleTask[] Tasks { get; set; }
  }

}
