using Newtonsoft.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods.UserPass;
using VaultSharp.V1.Commons;

namespace Netpack.Configuration.VaultHashiCorp
{
    internal class VaultHashiCorpSecretProvisioner : ISecretProvisioner
    {
        public string GetSecret(string connectionString, string? devkey, string key)
        {
            if (connectionString == "vault")
            {
                string secretContent = GetSecretContent(key);
                if (secretContent != null)
                {
                    VaultKvInfo? kvInfo = JsonConvert.DeserializeObject<VaultKvInfo>(secretContent);
                    if (kvInfo != null && kvInfo.Data.TryGetValue(key, out string? value))
                    {
                        return value;
                    }
                }

                throw new Exception($"Secret {key} not found");
            }

            ValidateDevKey(devkey);

            string[] slices = devkey.Split("@");

            UserPassAuthMethodInfo authMethod = new(slices[0], slices[1]);
            VaultClientSettings vaultSettings = new(connectionString, authMethod);

            VaultClient vaultClient = new(vaultSettings);
            Secret<SecretData> kv2Secret = vaultClient.V1.Secrets.KeyValue.V2
                               .ReadSecretAsync(path: "databases", mountPoint: "kv")
                               .Result;
            return kv2Secret.Data.Data.First(p => p.Key == key).Value.ToString();
        }

        private static string GetSecretContent(string key)
        {
            string keyPath = $"/vault/secrets/global.txt";
            string globalPath = $"/vault/secrets/{key}.txt";

            if (File.Exists(keyPath))
            {
                return File.ReadAllText(keyPath);
            }
            else if (File.Exists(globalPath))
            {
                return File.ReadAllText(globalPath);
            }

            return null;
        }

        private static void ValidateDevKey(string? devkey)
        {
            if (string.IsNullOrEmpty(devkey))
            {
                throw new Exception("Укажите секретный ключ для получения секретов (devkey)");
            }
        }

        public class VaultKvInfo
        {
            public Dictionary<string, string> Data { get; set; } = [];
        }
    }
}
