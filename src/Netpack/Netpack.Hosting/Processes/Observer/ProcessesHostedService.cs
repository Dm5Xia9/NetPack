using Microsoft.Extensions.Hosting;

namespace Netpack.Hosting.Processes.Observer
{
    public class ProcessesHostedService : IHostedService
    {
        private readonly IProcessStorage _processStorage;

        public ProcessesHostedService(IProcessStorage processStorage)
        {
            _processStorage = processStorage;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _processStorage.UnRegisterAll();
            return Task.CompletedTask;
        }
    }
}
