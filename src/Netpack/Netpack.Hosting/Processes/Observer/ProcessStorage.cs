using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Netpack.Hosting.Processes.Observer
{
    public class ProcessStorage : IProcessStorage
    {
        private readonly IOptions<ProcessObserverOptions> _options;
        private static readonly object _locker = new();
        private readonly List<Process> _processes = [];
        public ProcessStorage(IOptions<ProcessObserverOptions> options)
        {
            _options = options;
        }

        public IDisposable RegisterProcess(Process process)
        {
            lock (_locker)
            {
                HashSet<ObserverProcess> processes = [];
                string json;
                if (File.Exists(_options.Value.ProcessesFile))
                {
                    json = File.ReadAllText(_options.Value.ProcessesFile);
                    processes = JsonConvert.DeserializeObject<HashSet<ObserverProcess>>(json)!;
                }
                _ = processes.Add(new(process.Id));
                _processes.Add(process);
                json = JsonConvert.SerializeObject(processes);
                File.WriteAllText(_options.Value.ProcessesFile, json);
            }

            return new ProcessScope(this, process);
        }

        public void UnRegisterProcess(Process process)
        {
            lock (_locker)
            {

                HashSet<ObserverProcess> processes = [];
                string json;
                if (File.Exists(_options.Value.ProcessesFile))
                {
                    json = File.ReadAllText(_options.Value.ProcessesFile);
                    processes = JsonConvert.DeserializeObject<HashSet<ObserverProcess>>(json)!;
                }
                _ = processes.Remove(new(process.Id));
                _ = _processes.Remove(process);
                json = JsonConvert.SerializeObject(processes);
                File.WriteAllText(_options.Value.ProcessesFile, json);
            }
        }

        public void UnRegisterAll()
        {
            foreach (Process process in _processes)
            {
                process.Kill();
            }
        }

        public record ObserverProcess(int id);
    }
}
