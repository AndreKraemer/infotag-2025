using System.Text.Json.Serialization;

namespace MiracleListVueHybridDemo.Dtos
{
  public class MiracleTask
  {
    [JsonPropertyName("taskID")]
    public int TaskID { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("created")]
    public DateTime Created { get; set; }

    [JsonPropertyName("due")]
    public DateTime Due { get; set; }

    [JsonPropertyName("importance")]
    public int Importance { get; set; }

    [JsonPropertyName("note")]
    public string Note { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("effort")]
    public double Effort { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("dueInDays")]
    public int DueInDays { get; set; }

    [JsonPropertyName("subTaskSet")]
    public Subtaskset[] subTaskSet { get; set; }

    [JsonPropertyName("category")]
    public object Category { get; set; }

    [JsonPropertyName("categoryID")]
    public int CategoryID { get; set; }
  }

}
