namespace Netpack.Configuration
{
    public static class ExecutableConfigurationBuilder
    {
        internal static ExecutableConfiguration Build(ExecutableConfigurationDTO dto, string env, string? baseFolder = null)
        {
            ExecutableConfiguration config = new()
            {
                Environment = env,
                EnvironmentLabel = Environment.GetEnvironmentVariable(KnowEnvironmentVariables.EnvironmentLabel) ?? env,
                Name = dto.Name,
                Devkey = dto.Devkey,
                Version = dto.Version,
                Depends = dto.Depends,
                Provider = dto.Provider,
                Vars = dto.Vars.ToDictionary(p => VariableAnnotation.Parse(p.Key), p => p.Value),
            };
            config.Modules = dto.Modules
                    .ToDictionary(
                        p => p.Key,
                        p => (IModuleConfiguration)new ModuleConfiguration(p.Value, config));

            config.Modules ??= [];

            if (dto.Depends == null)
            {
                config.Depends = [];
            }

            if (dto.Envs.ContainsKey(env))
            {
                ExecutableConfigurationDTO envConfig = dto.Envs[env];

                if (string.IsNullOrEmpty(envConfig.TemplateFileName) == false)
                {
                    envConfig = ExecutableConfigurationDTO.ParseFromFile(envConfig.TemplateFileName, baseFolder);
                }

                if (string.IsNullOrEmpty(envConfig.Name) == false)
                {
                    config.Name = envConfig.Name;
                }

                if (string.IsNullOrEmpty(envConfig.Version) == false)
                {
                    config.Version = envConfig.Version;
                }

                if (string.IsNullOrEmpty(envConfig.Provider) == false)
                {
                    config.Provider = envConfig.Provider;
                }

                if (string.IsNullOrEmpty(envConfig.Devkey) == false)
                {
                    config.Devkey = envConfig.Devkey;
                }

                if (envConfig.Secrets != null && envConfig.Secrets.Count != 0)
                {
                    foreach (KeyValuePair<string, string> dep in envConfig.Secrets)
                    {
                        config.Secrets[dep.Key] = dep.Value;
                    }
                }

                if (envConfig.Vars != null && envConfig.Vars.Count != 0)
                {
                    foreach (KeyValuePair<string, string> dep in envConfig.Vars)
                    {
                        VariableAnnotation an = VariableAnnotation.Parse(dep.Key);

                        MergeVar(config, an, dep.Value);
                    }
                }

                if (envConfig.Depends != null && envConfig.Depends.Count != 0)
                {
                    foreach (KeyValuePair<string, string> dep in envConfig.Depends)
                    {
                        config.Depends[dep.Key] = dep.Value;
                    }
                }

                if (envConfig.Modules != null && envConfig.Modules.Count != 0)
                {
                    foreach (KeyValuePair<string, Dictionary<string, object>> dep in envConfig.Modules)
                    {
                        if (config.Modules.ContainsKey(dep.Key) == false)
                        {
                            continue;
                        }

                        foreach (KeyValuePair<string, object> prop in dep.Value)
                        {
                            ((ModuleConfiguration)config.Modules[dep.Key])
                                .MergeParameter(prop.Key, prop.Value);
                        }
                    }
                }
            }

            if (config.Devkey != null && Guid.TryParse(config.Devkey, out Guid id) == false)
            {
                config.Devkey = FindFileRecursive(config.Devkey, AppDomain.CurrentDomain.BaseDirectory);
            }

            config.EnvFile = FindFileRecursive(".env", AppDomain.CurrentDomain.BaseDirectory);

            if (string.IsNullOrEmpty(dto.BaseFileAlias) == false)
            {
                string result = GetPathFromAlias(dto.BaseFileAlias);
                if (result == null)
                {
                    throw new Exception($"Базовая конфигурация не найдена {dto.BaseFileAlias}");
                }

                ExecutableConfigurationDTO baseDto = ExecutableConfigurationDTO.ParseFromFile(result);
                string directoryPath = Path.GetDirectoryName(result)!;

                ExecutableConfiguration baseConfiguration = Build(baseDto, env, directoryPath);
                MergeConfigurations(config, baseConfiguration);
            }

            return config;
        }

        private static void MergeVar(ExecutableConfiguration config, VariableAnnotation annotation, string value)
        {
            VariableAnnotation exAn = config.Vars.FirstOrDefault(p => p.Key.Name == annotation.Name).Key;
            if (exAn != null)
            {
                exAn.Modifiers.UnionWith(annotation.Modifiers);
                if (string.IsNullOrEmpty(annotation.Alias) == false)
                {
                    exAn.Alias = annotation.Alias;
                }

                config.Vars[exAn] = value;
            }
            else
            {
                config.Vars[annotation] = value;
            }
        }

        public static void MergeConfigurations(ExecutableConfiguration origin, ExecutableConfiguration bs)
        {
            if (bs.Devkey != null && origin.Devkey == null)
            {
                origin.Devkey = bs.Devkey;
            }

            if (bs.Provider != null && origin.Provider == null)
            {
                origin.Provider = bs.Provider;
            }

            if (bs.Secrets != null && bs.Secrets.Count != 0)
            {
                foreach (KeyValuePair<string, string> dep in bs.Secrets)
                {
                    if (origin.Secrets.ContainsKey(dep.Key))
                    {
                        continue;
                    }

                    origin.Secrets[dep.Key] = dep.Value;
                }
            }

            if (bs.Vars != null && bs.Vars.Count != 0)
            {
                foreach (KeyValuePair<VariableAnnotation, string> dep in bs.Vars)
                {
                    if (origin.Vars.Any(p => p.Key.Name == dep.Key.Name))
                    {
                        continue;
                    }

                    MergeVar(origin, dep.Key, dep.Value);
                }
            }

            if (bs.Modules != null && bs.Modules.Count != 0)
            {
                foreach (KeyValuePair<string, IModuleConfiguration> dep in bs.Modules)
                {
                    if (origin.Modules.ContainsKey(dep.Key) == false)
                    {
                        origin.Modules[dep.Key] = dep.Value;
                        ((ModuleConfiguration)origin.Modules[dep.Key]).Configuration = origin;
                        continue;
                    }

                    ModuleConfiguration module = (ModuleConfiguration)dep.Value;
                    foreach (KeyValuePair<string, string> prop in module.Originals)
                    {
                        if (origin.Modules[dep.Key].Parameters.ContainsKey(prop.Key))
                        {
                            ((ModuleConfiguration)origin.Modules[dep.Key])
                                .MergeAnnotation(prop.Value);
                            continue;
                        }

                        object value = module.Parameters[prop.Key];

                        ((ModuleConfiguration)origin.Modules[dep.Key])
                            .MergeParameter(prop.Value, value);
                    }
                }
            }
        }

        private static string GetPathFromAlias(string alias)
        {
            string[] parts = alias.Split('>', StringSplitOptions.RemoveEmptyEntries);

            string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            string? result = null;
            for (int i = 0; i < parts.Length; i++)
            {
                if (currentFolder == null)
                {
                    break;
                }

                bool lastPart = i == parts.Length - 1;
                string part = parts[i].Trim();
                if (i % 2 == 1)
                {
                    if (lastPart)
                    {
                        result = FindFileInDirectory(part, currentFolder);
                    }
                    else
                    {
                        currentFolder = FindDirectoryInDirectory(part, currentFolder);
                    }
                }
                else
                {
                    if (lastPart)
                    {
                        result = FindFileRecursive(part, currentFolder);
                    }
                    else
                    {
                        currentFolder = FindDirectoryRecursive(part, currentFolder);
                    }
                }
            }

            return result ?? throw new Exception($"Последняя операция поиск по {currentFolder} не дала результат");
        }

        public static string? FindFileRecursive(string fileName, string folderPath)
        {
            // Проверяем существование папки
            if (!Directory.Exists(folderPath))
            {
                return null;
            }

            // Ищем файл в текущей папке
            string filePath = Path.Combine(folderPath, fileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }

            // Получаем родительскую папку
            string? parentFolder = Directory.GetParent(folderPath)?.FullName;

            // Если родительская папка существует, рекурсивно ищем в ней
            if (parentFolder != null)
            {
                string foundFile = FindFileRecursive(fileName, parentFolder);
                if (foundFile != null)
                {
                    return foundFile;
                }
            }

            // Файл не найден
            return null;
        }

        public static string? FindDirectoryRecursive(string fileName, string folderPath)
        {
            // Проверяем существование папки
            if (!Directory.Exists(folderPath))
            {
                return null;
            }

            // Ищем файл в текущей папке
            string filePath = Path.Combine(folderPath, fileName);
            if (Directory.Exists(filePath))
            {
                return filePath;
            }

            // Получаем родительскую папку
            string? parentFolder = Directory.GetParent(folderPath)?.FullName;

            // Если родительская папка существует, рекурсивно ищем в ней
            if (parentFolder != null)
            {
                string foundFile = FindDirectoryRecursive(fileName, parentFolder);
                if (foundFile != null)
                {
                    return foundFile;
                }
            }

            // Файл не найден
            return null;
        }

        private static string FindFileInDirectory(string fileName, string directoryPath)
        {
            string[] files = Directory.GetFiles(directoryPath, fileName, SearchOption.AllDirectories);
            return files.FirstOrDefault();
        }

        private static string FindDirectoryInDirectory(string folderName, string directoryPath)
        {
            string[] files = Directory.GetDirectories(directoryPath, folderName, SearchOption.AllDirectories);
            return files.FirstOrDefault();
        }
    }
}
