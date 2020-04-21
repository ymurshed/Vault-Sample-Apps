using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vault.Core.Helper;

namespace Vault.Core.Vault
{
    internal class VaultService : IVaultService
    {
        private readonly VaultSettings _vaultSettings;

        public VaultService(VaultSettings vaultSettings)
        {
            _vaultSettings = vaultSettings;
            LoadEnvironmentVariables();
        }

        public async Task<T> GetDefaultAsync<T>()
            => await GetAsync<T>(_vaultSettings.Key);

        public async Task<IDictionary<string, object>> GetDefaultAsync()
            => await GetAsync(_vaultSettings.Key);

        public async Task<T> GetAsync<T>(string key)
            => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(await GetAsync(key)));

        public async Task<IDictionary<string, object>> GetAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new VaultException("Vault secret key can not be empty.");            
            }
            try
            {
                var url = $"{_vaultSettings.Url}/{VaultConstants.BasePath}/{key}";
                var authHeaders = GetAuthMethod();
                var result = HttpClientHelper.GetAsync(url, authHeaders).GetAwaiter().GetResult();
                var jsonData = await result.Content.ReadAsStringAsync();
                var data = new JsonParser().GetData(jsonData);
                return data;
            }
            catch (Exception exception)
            {
                throw new VaultException($"Getting Vault secret for key: '{key}' caused an error. " +
                    $"{exception.Message}", exception, key);
            }
        }

        private IDictionary<string, string> GetAuthMethod()
        {
            var authHeader = new Dictionary<string, string>();
            
            switch (_vaultSettings.AuthType?.ToLowerInvariant())
            {
                case VaultConstants.TokenAuthType: 
                    authHeader.Add(VaultConstants.AuthHeaderKey, _vaultSettings.Token);
                    break;
                
                case VaultConstants.UserPassAuthType:
                    authHeader.Add(VaultConstants.AuthHeaderKey, _vaultSettings.Username);
                    break;
            }

            if (authHeader.Any()) return authHeader;
            throw new VaultAuthTypeNotSupportedException($"Vault auth type: '{_vaultSettings.AuthType}' is not supported.",
                _vaultSettings.AuthType);
        }

        private void LoadEnvironmentVariables()
        {
            _vaultSettings.Url = GetEnvironmentVariableValue("VAULT_URL") ?? _vaultSettings.Url;
            _vaultSettings.Key = GetEnvironmentVariableValue("VAULT_KEY") ?? _vaultSettings.Key;
            _vaultSettings.AuthType = GetEnvironmentVariableValue("VAULT_AUTH_TYPE") ?? _vaultSettings.AuthType;
            _vaultSettings.Token = GetEnvironmentVariableValue("VAULT_TOKEN") ?? _vaultSettings.Token;
            _vaultSettings.Username = GetEnvironmentVariableValue("VAULT_USERNAME") ?? _vaultSettings.Username;
            _vaultSettings.Password = GetEnvironmentVariableValue("VAULT_PASSWORD") ?? _vaultSettings.Password;
        }

        private static string GetEnvironmentVariableValue(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}