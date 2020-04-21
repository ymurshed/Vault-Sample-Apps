using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vault.Core.Vault
{
    internal interface IVaultService
    {
        Task<T> GetDefaultAsync<T>();
        Task<IDictionary<string, object>> GetDefaultAsync();
        Task<T> GetAsync<T>(string key);
        Task<IDictionary<string, object>> GetAsync(string key);
    }
}