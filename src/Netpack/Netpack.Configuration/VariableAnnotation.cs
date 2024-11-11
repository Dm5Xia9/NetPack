using System.Text.RegularExpressions;

namespace Netpack.Configuration
{
    public class VariableAnnotation
    {
        public HashSet<string> Modifiers { get; private set; } = [];
        public string Name { get; private set; } = string.Empty;
        public string? Alias { get; set; }
        public bool? Nullable { get; private set; }
        public required string Original { get; set; }
        private static readonly Regex _startString = new("\\((.*?)\\)([?!])|(^\\S+)([?!])");
        private static readonly Regex _aliasRegex = new("\\[(.*?)\\]");

        public static VariableAnnotation Parse(string variable)
        {
            VariableAnnotation annotation = new() { Original = variable };

            Match match = _startString.Match(variable);
            if (match.Success)
            {
                if (match.Groups[1].Success)
                {
                    annotation.Modifiers = match.Groups[1].Value.Split(' ').ToHashSet();
                }

                if (match.Groups[3].Success)
                {
                    _ = annotation.Modifiers.Add(match.Groups[1].Value);
                }

                string? r = null;

                if (match.Groups[2].Success)
                {
                    r = match.Groups[2].Value;
                }

                if (match.Groups[4].Success)
                {
                    r = match.Groups[6].Value;
                }

                if (r != null)
                {
                    annotation.Nullable = r == "?";
                }
            }

            variable = _startString.Replace(variable, "")
                .Trim();

            match = _aliasRegex.Match(variable);
            if (match.Success)
            {
                if (match.Groups[1].Success)
                {
                    annotation.Alias = match.Groups[1].Value;
                    if (annotation.Alias.Contains(" "))
                    {
                        throw new Exception($"Алиас {annotation.Alias} не может иметь пробел ");
                    }
                }
            }

            variable = _aliasRegex.Replace(variable, "")
               .Trim();

            if (variable.Contains(" "))
            {
                throw new Exception($"Конфигурация {variable} не может иметь пробел ");
            }

            annotation.Name = variable;
            return annotation;
        }
    }
}
