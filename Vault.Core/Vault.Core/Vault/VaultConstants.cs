namespace Vault.Core.Vault
{
    internal class VaultConstants
    {
        public static string VaultSettingsKey = "VaultSettings";
        public static string BasePath = "v1/secret/data";

        public const string AuthHeaderKey = "X-Vault-Token";
        public const string TokenAuthType = "token";
        public const string UserPassAuthType = "userpass";
    }
}
