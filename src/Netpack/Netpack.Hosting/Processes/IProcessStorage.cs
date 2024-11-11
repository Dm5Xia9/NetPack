using System.Diagnostics;

namespace Netpack.Hosting.Processes
{
    public interface IProcessStorage
    {
        IDisposable RegisterProcess(Process process);
        void UnRegisterProcess(Process process);
        void UnRegisterAll();
    }
}