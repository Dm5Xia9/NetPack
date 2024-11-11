using System.Diagnostics;

namespace Netpack.Hosting.Processes.Observer
{
    public class ProcessScope : IDisposable
    {
        private readonly ProcessStorage _processStorage;
        private readonly Process _process;

        public ProcessScope(ProcessStorage processStorage, Process process)
        {
            _processStorage = processStorage;
            _process = process;
        }

        public void Dispose()
        {
            _processStorage.UnRegisterProcess(_process);
            _process.Kill();
        }
    }
}
