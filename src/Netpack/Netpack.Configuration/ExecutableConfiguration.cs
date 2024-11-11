namespace Netpack.Configuration
{
    public class ExecutableConfiguration : IExecutableConfiguration
    {
        private readonly VariablesMachine _variablesMachine = new();
        private EnvFileConfiguration? _envFileConfiguration;
        internal ExecutableConfiguration()
        {
            _variablesMachine.AddResolver(System.Environment.GetEnvironmentVariable);
        }

        public string? EnvFile { get; set; }
        public required string Environment { get; set; }
        public required string EnvironmentLabel { get; set; }
        public required string Name { get; set; }
        public required string Version { get; set; }
        public string? Devkey { get; set; }
        public required string Provider { get; set; }
        public Dictionary<VariableAnnotation, string> Vars { get; set; } = [];

        public Dictionary<string, string> Depends { get; set; } = [];
        public Dictionary<string, string> Secrets { get; set; } = [];

        public Dictionary<string, IModuleConfiguration> Modules { get; set; } = [];

        IDictionary<string, string> IExecutableConfiguration.Depends => Depends;

        IDictionary<string, IModuleConfiguration> IExecutableConfiguration.Modules => Modules;
        public static ExecutableConfiguration Create(string file, string env)
        {
            ExecutableConfigurationDTO dto = ExecutableConfigurationDTO.ParseFromFile(file);
            return ExecutableConfigurationBuilder.Build(dto, env);
        }

        public string GetVariable(string name)
        {
            return _variablesMachine.GetValue(name);
        }

        internal void PostInitialize()
        {
            if (string.IsNullOrEmpty(EnvFile) == false)
            {
                _envFileConfiguration = new EnvFileConfiguration(EnvFile);
                _variablesMachine.AddResolver(p =>
                {
                    return _envFileConfiguration!.GetValue(p);
                });
            }

            foreach (KeyValuePair<VariableAnnotation, string> v in Vars)
            {
                _variablesMachine.AddVariableLiteral(v.Key.Name, v.Value);

                if (v.Key.Alias != null)
                {
                    _variablesMachine.SetAlternativeName(v.Key.Name, v.Key.Alias);
                }
            }

            _variablesMachine.AddVariableLiteral(KnowEnvironmentVariables.UpperName, Name.ToUpper());
            _variablesMachine.AddVariableLiteral(KnowEnvironmentVariables.Name, Name);
            _variablesMachine.AddVariableLiteral(KnowEnvironmentVariables.Version, Version);
            _variablesMachine.AddVariableLiteral(KnowEnvironmentVariables.SecretProvider, Provider);

            foreach (KeyValuePair<string, IModuleConfiguration> module in Modules.ToList())
            {
                foreach (KeyValuePair<string, object> param in module.Value.Parameters)
                {
                    string k = $"{module.Key}_{param.Key}";
                    _variablesMachine.AddVariableLiteral(k, param.Value?.ToString() ?? string.Empty);

                    VariableAnnotation annotation = module.Value.Annotations[param.Key];
                    if (annotation.Alias != null)
                    {
                        _variablesMachine.SetAlternativeName(k, annotation.Alias);
                    }
                }
            }

            Name = PostProcessVariable(KnowEnvironmentVariables.Name);
            Version = PostProcessVariable(KnowEnvironmentVariables.Version);
            Provider = PostProcessVariable(KnowEnvironmentVariables.SecretProvider);

            foreach (KeyValuePair<string, string> depend in Depends.ToList())
            {
                Depends[depend.Key] = depend.Value;
            }

            foreach (KeyValuePair<string, IModuleConfiguration> module in Modules.ToList())
            {
                ModuleConfiguration internalModule = (ModuleConfiguration)module.Value;
                foreach (KeyValuePair<string, object> param in internalModule.Parameters)
                {
                    if (param.Value is string or int or bool or null)
                    {
                        string k = $"{module.Key}_{param.Key}";
                        internalModule.SetParameterValue(param.Key, PostProcessVariable(k, Provider));

                    }
                }
            }

            _variablesMachine.PrintDebug();
        }

        private string PostProcessVariable(string key, string? secretConnectionString = null)
        {
            string envKey = $"{key.Replace(".", "_").Replace("-", "_").ToUpper()}_CONFIG";
            string? envValue = System.Environment.GetEnvironmentVariable(envKey);
            if (envValue != null)
            {
                return envValue;
            }

            string? v = _variablesMachine.GetValue(key);
            if (v.StartsWith("<< ") && secretConnectionString != null)
            {
                string secretKey = v[3..];
                if (Secrets.ContainsKey(secretKey))
                {
                    return Secrets[secretKey];
                }

                string secret = SecretProvisioner.Instance
                    .GetSecret(secretConnectionString, Devkey, secretKey);
                return secret ??
                    throw new Exception($"Секрет {secretKey} не найден");
            }

            return v;
        }
    }
}
