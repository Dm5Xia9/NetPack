using Microsoft.Extensions.Options;
using Netpack.Hosting.Processes.Observer;
using System.Diagnostics;

namespace Netpack.Hosting.Processes.Observer
{
    public class ProcessObserver
    {
        private readonly IOptions<ProcessObserverOptions> _options;
        private readonly string _exeName = "Netpack.Hosting.ProcessObserver.exe";
        public ProcessObserver(IOptions<ProcessObserverOptions> options)
        {
            _options = options;
        }

        public void Start()
        {
            Process currentProcess = Process.GetCurrentProcess();

            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _exeName),
                    Arguments = $"{currentProcess.Id} \"{_options.Value.WorkFolderPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            };

            process.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) { Console.WriteLine(e.Data); } };
            process.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) { Console.WriteLine($"Error: {e.Data}"); } };
            _ = process.Start();
        }
    }
}
