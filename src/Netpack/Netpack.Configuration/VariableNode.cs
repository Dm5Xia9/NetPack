namespace Netpack.Configuration
{
    internal class VariableNode
    {
        public string Name { get; }
        public string OriginalString { get; set; }
        public VariableNode? AlternativeName { get; set; }
        public HashSet<VariableNode> Dependencies { get; }
        public HashSet<string> UnresolvedVariables { get; }

        public VariableNode(string name, string originalString)
        {
            Name = name;
            OriginalString = originalString;
            Dependencies = [];
            UnresolvedVariables = [];
        }

        public void AddDependency(VariableNode dependency)
        {
            _ = Dependencies.Add(dependency);
        }

        public void AddUnresolvedVariable(string variable)
        {
            _ = UnresolvedVariables.Add(variable);
        }

        public string GetStringWithVariables(Func<string, string?>? rezolver = null, HashSet<VariableNode>? usedNodes = null)
        {
            usedNodes ??= [];
            _ = usedNodes.Add(this);

            if (AlternativeName != null)
            {
                string altKey = AlternativeName.GetStringWithVariables(rezolver, usedNodes);
                string? altValue = rezolver?.Invoke(altKey);
                if (altValue != null)
                {
                    return altValue;
                }
            }

            string result = rezolver?.Invoke(Name) ?? OriginalString;

            foreach (VariableNode dependency in Dependencies)
            {
                if (usedNodes.Contains(dependency))
                {
                    throw new Exception($"Рекурсивный вызов переменной {dependency.Name}");
                }

                string? value = rezolver?.Invoke(dependency.Name);
                value ??= dependency.GetStringWithVariables(rezolver, usedNodes);
                result = result.Replace($"${dependency.Name}", value);
                result = result.Replace($"${{{dependency.Name}}}", value);

                _ = usedNodes.Add(dependency);
            }

            // Заменяем незарезолвенные переменные на пустые значения
            foreach (string unresolved in UnresolvedVariables)
            {
                string? value = rezolver?.Invoke(unresolved);
                result = result.Replace($"${unresolved}", value ?? string.Empty);
                result = result.Replace($"${{{unresolved}}}", value ?? string.Empty);
            }

            return result;
        }
    }
}
