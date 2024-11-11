namespace Netpack.Configuration
{
    public interface IModuleConfiguration
    {
        bool Active { get; }
        IExecutableConfiguration Configuration { get; }
        IReadOnlyDictionary<string, object> Parameters { get; }
        IReadOnlyDictionary<string, VariableAnnotation> Annotations { get; }
        string GetOrDefault(string name, string def);
        string GetRequiredParameter(string name);
        void RequiredParameter(string name);
        IModuleConfiguration PartitionBy(string token);
    }
}