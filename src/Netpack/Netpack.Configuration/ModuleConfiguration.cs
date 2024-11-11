namespace Netpack.Configuration
{
    internal class ModuleConfiguration : IModuleConfiguration
    {
        private readonly Dictionary<string, VariableAnnotation> _annotations = [];
        private readonly Dictionary<string, object> _parameters = [];
        private readonly Dictionary<string, string> _originals = [];

        public ModuleConfiguration(Dictionary<string, object> parameters, IExecutableConfiguration configuration)
        {
            Dictionary<VariableAnnotation, KeyValuePair<string, object>> all = parameters.ToDictionary(p => VariableAnnotation.Parse(p.Key));
            _annotations = all.ToDictionary(p => p.Key.Name, p => p.Key);
            _parameters = all.ToDictionary(p => p.Key.Name, p => p.Value.Value);
            Configuration = configuration;
            _originals = all.ToDictionary(p => p.Key.Name, p => p.Value.Key);
        }

        public bool Active => GetOrDefault("active", "true") == "true";
        public IReadOnlyDictionary<string, object> Parameters => _parameters;

        public IExecutableConfiguration Configuration { get; set; }
        public IReadOnlyDictionary<string, string> Originals => _originals;
        public IReadOnlyDictionary<string, VariableAnnotation> Annotations => _annotations;

        public void MergeParameter(string key, object value)
        {
            VariableAnnotation annotration = VariableAnnotation.Parse(key);

            VariableAnnotation exAnnotation = _annotations.FirstOrDefault(p => p.Key == annotration.Name).Value;
            if (exAnnotation != null)
            {
                exAnnotation.Modifiers.UnionWith(annotration.Modifiers);
                if (string.IsNullOrEmpty(annotration.Alias) == false)
                {
                    exAnnotation.Alias = annotration.Alias;
                }
            }
            else
            {
                _annotations[annotration.Name] = annotration;
            }

            _parameters[annotration.Name] = value;
            _originals[annotration.Name] = key;
        }

        public void MergeAnnotation(string key)
        {
            VariableAnnotation annotration = VariableAnnotation.Parse(key);

            VariableAnnotation exAnnotation = _annotations.FirstOrDefault(p => p.Key == annotration.Name).Value;
            if (exAnnotation != null)
            {
                exAnnotation.Modifiers.UnionWith(annotration.Modifiers);
                if (string.IsNullOrEmpty(annotration.Alias) == false)
                {
                    exAnnotation.Alias = annotration.Alias;
                }
            }
            else
            {
                _annotations[annotration.Name] = annotration;
            }
        }

        public void SetParameterValue(string key, object value)
        {
            _parameters[key] = value;
        }

        public string? GetRequiredParameter(string name)
        {
            string envKey = $"VAR_{name.Replace(".", "_").Replace("-", "_").ToUpper()}";
            string? envValue = Environment.GetEnvironmentVariable(envKey);
            return envValue != null
                ? envValue
                : _parameters.ContainsKey(name)
                ? _parameters[name].ToString()
                : throw new InvalidOperationException($"Требуется обязательный параметр {name}");
        }

        public string? GetOrDefault(string name, string def)
        {
            string envKey = $"VAR_{name.Replace(".", "_").Replace("-", "_").ToUpper()}";
            string? envValue = Environment.GetEnvironmentVariable(envKey);
            return envValue != null ? envValue : _parameters.ContainsKey(name) ? _parameters[name].ToString() : def;
        }

        public void RequiredParameter(string name)
        {
            if (!_parameters.ContainsKey(name))
            {
                throw new InvalidOperationException($"Требуется обязательный параметр {name}");
            }
        }

        public IModuleConfiguration PartitionBy(string token)
        {
            string tokenSubString = $"{token}.";
            Dictionary<string, object> subParameters = _parameters
                .Where(p => p.Key.StartsWith(tokenSubString))
                .ToDictionary(
                    p => p.Key.Replace(tokenSubString, string.Empty),
                    p => p.Value);

            ModuleConfiguration module = new(subParameters, Configuration);
            return module;
        }
    }
}
