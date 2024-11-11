using Aspire.Hosting.ApplicationModel;

namespace Netpack.Hosting.Models
{
    public interface INetpackProvisioner
    {
        Task BeforeStartAsync(IResourceCollection appModel, CancellationToken cancellationToken = default);
        Task AfterEndpointsAllocatedAsync(IResourceCollection appModel, CancellationToken cancellationToken = default);
        Task AfterResourcesCreatedAsync(IResourceCollection appModel, CancellationToken cancellationToken = default);
    }
}
