using System.Text;
using System.Text.RegularExpressions;

namespace Netpack.Configuration
{
    internal class VariablesGraph
    {
        private readonly Dictionary<string, VariableNode> nodes;

        public VariablesGraph()
        {
            nodes = [];
        }

        public VariableNode GetOrCreateNode(string name, string originalString)
        {
            if (nodes.TryGetValue(name, out VariableNode? existingNode))
            {
                return existingNode;
            }
            else
            {
                // Создаем новую ноду
                VariableNode newNode = new(name, originalString);
                nodes[name] = newNode;
                return newNode;
            }
        }

        public VariableNode? GetNode(string name)
        {
            return nodes.ContainsKey(name) ? nodes[name] : null;
        }

        public void AddVariable(string variable, string literal)
        {
            VariableNode currentNode = GetOrCreateNode(variable, literal);
            currentNode.OriginalString = literal;

            UpdateDependencies();

            TestRun();
        }

        private void UpdateDependencies()
        {
            // Пересобираем зависимости для всех узлов, которые ссылаются на обновленную переменную
            foreach (VariableNode node in nodes.Values)
            {
                node.Dependencies.Clear();
                node.UnresolvedVariables.Clear();

                HashSet<string> dependencies = ExtractVariables(node.OriginalString);
                foreach (string dep in dependencies)
                {
                    if (nodes.ContainsKey(dep))
                    {
                        node.AddDependency(nodes[dep]);
                    }
                    else
                    {
                        node.AddUnresolvedVariable(dep);
                    }
                }
            }
        }

        /// <summary>
        /// необходимо для того чтобы превентивать ошибки.
        /// </summary>
        private void TestRun()
        {
            foreach (VariableNode node in nodes.Values)
            {
                // получаем значение без внешнего резолвера. Только граф
                _ = node.GetStringWithVariables();
            }
        }

        private HashSet<string> ExtractVariables(string input)
        {
            HashSet<string> variables = new();
            Regex regex = new(@"\$([A-Za-z_][A-Za-z0-9_]*)|\${([A-Za-z_][A-Za-z0-9_]*)}");
            MatchCollection matches = regex.Matches(input);

            foreach (Match match in matches)
            {
                if (match.Groups[1].Success)
                {
                    _ = variables.Add(match.Groups[1].Value);
                }
                else if (match.Groups[2].Success)
                {
                    _ = variables.Add(match.Groups[2].Value);
                }
            }

            return variables;
        }

        public void PrintGraph(Func<string, string?>? rezolver = null)
        {
            string text = ConvertDictionaryToString(nodes.Values.ToDictionary(p => p.Name, p => p.GetStringWithVariables(rezolver)));
            Console.WriteLine(text);
        }

        public static string ConvertDictionaryToString(Dictionary<string, string> dictionary)
        {
            int maxKeyLength = 0;
            foreach (string key in dictionary.Keys)
            {
                if (key.Length > maxKeyLength)
                {
                    maxKeyLength = key.Length;
                }
            }

            StringBuilder sb = new();
            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                string key = kvp.Key;
                string value = kvp.Value;
                int padding = maxKeyLength - key.Length;
                _ = sb.AppendLine($"{key}{new string(' ', padding)} : {value}");
            }

            return sb.ToString();
        }

        private IEnumerable<string> GetDependencyNames(VariableNode node)
        {
            foreach (VariableNode dependency in node.Dependencies)
            {
                yield return dependency.Name;
            }
        }
    }
}
