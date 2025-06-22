using System.Text.Json.Serialization;

namespace MiracleListVueHybridDemo.Dtos
{
  public class Subtaskset
  {
    [JsonPropertyName("subTaskID")]
    public int SubTaskID { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("task")]
    public MiracleTask Task { get; set; }

    [JsonPropertyName("taskID")]
    public int TaskID { get; set; }
  }

}
