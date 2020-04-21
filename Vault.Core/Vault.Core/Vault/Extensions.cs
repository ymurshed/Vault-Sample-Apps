using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Vault.Core.Helper;

namespace Vault.Core.Vault
{
    public static class Extensions
    {
        public static IWebHostBuilder UseVault(this IWebHostBuilder builder, string key = null)
            => builder.ConfigureServices(services => 
            {
                IConfiguration configuration;
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    configuration = serviceProvider.GetService<IConfiguration>();
                }

                var vaultSettings = configuration.GetOptions<VaultSettings>(VaultConstants.VaultSettingsKey);
                services.AddSingleton(vaultSettings);
                services.AddTransient<IVaultService, VaultService>();
            })
            .ConfigureAppConfiguration((ctx, cfg) =>
            {

            var vaultSettings = cfg.Build().GetOptions<VaultSettings>(VaultConstants.VaultSettingsKey);
            var enabled = vaultSettings.Enabled;
            var vaultEnabled = Environment.GetEnvironmentVariable("VAULT_ENABLED")?.ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(vaultEnabled))
            {
                enabled = vaultEnabled == "true" || vaultEnabled == "1";
            }
            if (enabled)
            {
                cfg.AddVault(vaultSettings, key);
            }
            });

        private static IConfigurationBuilder AddVault(this IConfigurationBuilder builder, VaultSettings vaultSettings, string key)
        {
            var client = new VaultService(vaultSettings);
            var secretSettings = string.IsNullOrWhiteSpace(key) ? 
                                    client.GetDefaultAsync().GetAwaiter().GetResult() : 
                                    client.GetAsync(key).GetAwaiter().GetResult();
            
            var parser = new JsonParser();
            var data = parser.Parse(JObject.FromObject(secretSettings));
            var source = new MemoryConfigurationSource {InitialData = data};
            builder.Add(source);
            return builder;
        }
    }
}