using DontLetMeExpireAvalonia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DontLetMeExpireAvalonia.Services;

public interface IStorageLocationService
{
    Task<IEnumerable<StorageLocation>> GetAsync();
    Task<IEnumerable<StorageLocationWithItemCount>> GetWithItemCountAsync();
    Task<StorageLocation?> GetByIdAsync(string id);
    Task SaveAsync(StorageLocation storageLocation);
    Task DeleteAsync(StorageLocation storageLocation);
    Task DeleteAllAsync();
}