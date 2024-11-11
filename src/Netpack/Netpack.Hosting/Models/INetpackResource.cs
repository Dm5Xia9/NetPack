using Aspire.Hosting.ApplicationModel;
using Netpack.Hosting.Orcestraction;

namespace Netpack.Hosting.Models
{
    public interface INetpackResource : IResource
    {
        public Task BeforeStartAsync(ResourceState state, CancellationToken cancellationToken = default);
        public Task AfterEndpointsAllocatedAsync(ResourceState state, CancellationToken cancellationToken = default);
        public Task AfterResourcesCreatedAsync(ResourceState state, CancellationToken cancellationToken = default);

    }

}
