using System.Text.Json.Serialization;

namespace MiracleListVueHybridDemo.Dtos
{
  public class Result
  {
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
  }
  public class Result<T> : Result
  {
    [JsonPropertyName("data")]
    public T Data { get; set; }
  }

}
