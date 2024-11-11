namespace Netpack.Configuration
{
    internal class EnvFileConfiguration
    {
        private readonly string _fileName;
        private Dictionary<string, string> _variables = [];
        private readonly FileSystemWatcher _watcher;

        public EnvFileConfiguration(string fileName)
        {
            _fileName = fileName;
            _variables = ParseEnvFile(fileName);
            _watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(fileName),
                Filter = Path.GetFileName(fileName),
                NotifyFilter = NotifyFilters.LastWrite
            };

            _watcher.Changed += Watcher_Changed;
            _watcher.EnableRaisingEvents = true;
        }

        public static event Action OnEnvChanged;

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _variables = ParseEnvFile(e.FullPath);
            OnEnvChanged?.Invoke();
        }

        public string? GetValue(string value)
        {
            return _variables.ContainsKey(value) ? _variables[value] : null;
        }

        private static Dictionary<string, string> ParseEnvFile(string filePath)
        {
            Dictionary<string, string> envVariables = [];

            foreach (string line in File.ReadLines(filePath))
            {
                // Игнорируем пустые строки и комментарии
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                // Разделяем строку на ключ и значение
                string[] parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim().Trim('\"'); // Убираем кавычки вокруг значения
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        envVariables[key] = value;
                    }
                }
            }

            return envVariables;
        }
    }
}
