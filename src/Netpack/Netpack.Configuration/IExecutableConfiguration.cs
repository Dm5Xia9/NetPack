namespace Netpack.Configuration
{
    public interface IExecutableConfiguration
    {
        public string Environment { get; }
        public string EnvironmentLabel { get; }
        public string Name { get; }
        public string Version { get; }
        public string? Devkey { get; }
        public string Provider { get; }
        public IDictionary<string, string> Depends { get; }
        public IDictionary<string, IModuleConfiguration> Modules { get; }
        public string GetVariable(string name);
    }
}
