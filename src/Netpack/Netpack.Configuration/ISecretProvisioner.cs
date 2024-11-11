namespace Netpack.Configuration
{
    public interface ISecretProvisioner
    {
        string GetSecret(string connectionString, string? devkey, string key);
    }

    public static class SecretProvisioner
    {
        public static void Register(ISecretProvisioner secretProvisioner)
        {
            Instance = secretProvisioner;
        }

        public static ISecretProvisioner? Instance { get; private set; }
    }
}
