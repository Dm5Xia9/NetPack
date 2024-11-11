namespace Netpack.Configuration.VaultHashiCorp
{
    public static class NetpackConfigurationExtensions
    {
        public static NetpackConfiguration WithVaultHashiCorp(this NetpackConfiguration netpackConfiguration)
        {
            return netpackConfiguration.WithSecretProvisioner(new VaultHashiCorpSecretProvisioner());
        }
    }
}
