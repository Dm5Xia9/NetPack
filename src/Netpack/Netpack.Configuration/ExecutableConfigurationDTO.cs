using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Netpack.Configuration
{
    internal class ExecutableConfigurationDTO
    {
        [YamlMember(Alias = "<<")]
        public string? TemplateFileName { get; set; }

        [YamlMember(Alias = ".")]
        public string? BaseFileAlias { get; set; }

        public required string Name { get; set; }
        public required string Version { get; set; }
        public string? Devkey { get; set; }
        public required string Provider { get; set; }
        public Dictionary<string, string> Depends { get; set; } = [];
        public Dictionary<string, string> Secrets { get; set; } = [];
        public Dictionary<string, string> Vars { get; set; } = [];
        public Dictionary<string, Dictionary<string, object>> Modules { get; set; } = [];
        public Dictionary<string, ExecutableConfigurationDTO> Envs { get; set; } = [];

        public static ExecutableConfigurationDTO ParseFromFile(string fileName, string? baseFolder = null)
        {
            string text;
            string file = File.Exists(fileName)
                ? fileName
                : Directory.GetFiles(baseFolder ?? AppDomain.CurrentDomain.BaseDirectory, fileName)
                    .First();
            using (StreamReader reader = new(file))
            {
                text = reader.ReadToEnd();
            }

            return ParseYamlFile(text);
        }

        private static ExecutableConfigurationDTO ParseYamlFile(string yml)
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
            .Build();

            ExecutableConfigurationDTO dependencyObject = deserializer.Deserialize<ExecutableConfigurationDTO>(yml);
            return dependencyObject;
        }
    }
}
