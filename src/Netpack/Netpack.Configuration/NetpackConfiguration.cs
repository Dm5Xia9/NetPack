namespace Netpack.Configuration
{
    public class NetpackConfiguration
    {
        private readonly string _file;
        private readonly string _env;
        private ISecretProvisioner? _secretProvisioner;
        public NetpackConfiguration(string file, string? env = null)
        {
            _file = file;
            _env = env ?? Environment
                .GetEnvironmentVariable(KnowEnvironmentVariables.PlatformEnvironment) ?? "local";
        }

        public NetpackConfiguration WithSecretProvisioner(ISecretProvisioner secretProvisioner)
        {
            _secretProvisioner = secretProvisioner;
            return this;
        }


        public IExecutableConfiguration Build()
        {
            if (_secretProvisioner == null)
            {
                throw new InvalidOperationException("SecretProvisioner not register");
            }

            SecretProvisioner.Register(_secretProvisioner);
            return ExecutableConfiguration.Create(_file, _env);
        }
    }
}
