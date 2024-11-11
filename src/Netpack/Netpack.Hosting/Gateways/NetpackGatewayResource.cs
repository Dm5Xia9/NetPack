using Aspire.Hosting.ApplicationModel;
using Netpack.Hosting.Clusters;
using Netpack.Hosting.Gateways.Models;
using Netpack.Hosting.Models;
using Netpack.Hosting.Orcestraction;

namespace Netpack.Hosting.Gateways
{
    public class NetpackGatewayResource : NetpackResource
    {
        private readonly List<NetpackClusterResource> _clusters = [];
        private GatewayClient _gatewayClient;

        public NetpackGatewayResource(string name) : base(name)
        {
        }

        public NetpackGatewayAvailableProvisions Provisions { get; private set; }

        public override Task AfterEndpointsAllocatedAsync(ResourceState state, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override async Task AfterResourcesCreatedAsync(ResourceState state, CancellationToken cancellationToken = default)
        {
            await state.SetState(KnownResourceStateStyles.Info, "Подключение");
            await using IAsyncDisposable scope = state.Scope();
            _gatewayClient = new GatewayClient();
            Provisions = GatewayClient.GetProvisions();
        }

        public override Task BeforeStartAsync(ResourceState state, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void RegisterCluster(NetpackClusterResource cluster)
        {
            _clusters.Add(cluster);
        }
    }



}
