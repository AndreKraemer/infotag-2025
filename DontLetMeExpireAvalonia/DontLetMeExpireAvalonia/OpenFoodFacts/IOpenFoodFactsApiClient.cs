using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.OpenFoodFacts;

public interface IOpenFoodFactsApiClient
{
  Task<byte[]> DownloadImage(string imageUrl);
  Task<ProductApiResponse> GetProductByCodeAsync(string code);
}