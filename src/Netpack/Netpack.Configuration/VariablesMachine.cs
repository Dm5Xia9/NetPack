namespace Netpack.Configuration
{
    internal class VariablesMachine
    {
        private readonly Dictionary<string, string> _values = [];

        private readonly List<Func<string, string>> _resolvers = [];

        private readonly VariablesGraph _variablesGraph = new();

        public void AddResolver(Func<string, string> resolver)
        {
            _resolvers.Add(resolver);
        }

        public void AddVariableLiteral(string variable, string literal)
        {
            literal ??= string.Empty;
            _variablesGraph.AddVariable(variable, literal);
        }

        public string? GetValue(string variable)
        {
            VariableNode? node = _variablesGraph.GetNode(variable);

            return node?.GetStringWithVariables(ResolveVariable);
        }

        public void SetAlternativeName(string variable, string altName)
        {
            string altNameKey = $"{variable}_ALT";
            _variablesGraph.AddVariable(altNameKey, altName);

            VariableNode? altNode = _variablesGraph.GetNode(altNameKey);
            VariableNode? node = _variablesGraph.GetNode(variable);
            if (node != null)
            {
                node.AlternativeName = altNode;
            }
        }

        public void PrintDebug()
        {
            _variablesGraph.PrintGraph(ResolveVariable);
        }

        private string? ResolveVariable(string key)
        {
            foreach (Func<string, string> resolver in _resolvers)
            {
                string v = resolver.Invoke(key);
                if (v != null)
                {
                    return v;
                }
            }

            return null;
        }
    }
}
