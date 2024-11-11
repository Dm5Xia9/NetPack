using Microsoft.Extensions.Logging;
using Netpack.Hosting.Processes;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Netpack.Hosting.Kubernetes
{
    public class Kubectl
    {
        private readonly string _path;
        private readonly string _basePath;
        private readonly IProcessStorage _storage;

        private string _kubeconfig;

        public Kubectl(string path, string basePath, IProcessStorage storage)
        {
            _path = path;
            _basePath = basePath;
            _storage = storage;
        }


        public void SetKubeconfig(string path)
        {
            _kubeconfig = Path.Combine(_basePath, "kubeconfig");
            File.Copy(path, _kubeconfig, true);
        }

        public async Task ForwardPort(string podName, string namespaceName, int internalPort, int externalPort)
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _path,
                    Arguments = $"--kubeconfig={_kubeconfig} port-forward --namespace {namespaceName} pod/{podName} {externalPort}:{internalPort}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) { Console.WriteLine(e.Data); } };
            process.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) { Console.WriteLine($"Error: {e.Data}"); } };
            _ = process.Start();

            using IDisposable scope = _storage.RegisterProcess(process);

            Console.WriteLine($"Форвардинг порта {externalPort} -> {internalPort} для пода {podName} в неймспейсе {namespaceName}...");
            await process.WaitForExitAsync();
        }

        public static async Task<Kubectl> DownloadIfNotExists(ILogger logger, IProcessStorage storage, CancellationToken cancellationToken)
        {
            string commonpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            string netpackPath = Path.Combine(commonpath, "netpack");
            if (!Directory.Exists(netpackPath))
            {
                _ = Directory.CreateDirectory(netpackPath);
            }

            string path = Path.Combine(netpackPath, "kubectl");

            if (!File.Exists(path))
            {
                logger.LogInformation("Скачивание kubectl...");
                using (HttpClient client = new())
                {
                    string url = GetKubectlDownloadUrl();
                    HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
                    _ = response.EnsureSuccessStatusCode();

                    await using FileStream fs = new(path, FileMode.CreateNew);
                    await response.Content.CopyToAsync(fs, cancellationToken);
                }

                // Делаем файл исполняемым (только для Unix-систем)
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process process = new()
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "chmod",
                            Arguments = $"+x {path}",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        }
                    };
                    _ = process.Start();
                    await process.WaitForExitAsync(cancellationToken);
                }

                logger.LogInformation("kubectl успешно скачан.");
            }
            else
            {
                logger.LogInformation("kubectl уже существует.");
            }

            return new Kubectl(path, commonpath, storage);
        }

        private static string GetKubectlDownloadUrl()
        {
            const string Version = "v1.31.0";
            string os = RuntimeInformation.OSDescription.ToLowerInvariant();
            _ = RuntimeInformation.OSArchitecture == Architecture.X64 ? "amd64" : "arm64";

            if (os.Contains("windows"))
            {
                return $"https://dl.k8s.io/release/{Version}/bin/windows/amd64/kubectl.exe";
            }
            else if (os.Contains("linux"))
            {
                return $"https://dl.k8s.io/release/{Version}/bin/linux/amd64/kubectl";
            }
            else if (os.Contains("darwin"))
            {
                return $"https://dl.k8s.io/release/{Version}/bin/darwin/amd64/kubectl";
            }

            throw new PlatformNotSupportedException("Операционная система не поддерживается.");
        }
    }


}
