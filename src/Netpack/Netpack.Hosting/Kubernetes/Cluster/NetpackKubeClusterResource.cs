using Aspire.Hosting.ApplicationModel;
using k8s;
using Netpack.Hosting.Clusters;
using Netpack.Hosting.Gateways;
using Netpack.Hosting.Gateways.Models;
using Netpack.Hosting.Orcestraction;

namespace Netpack.Hosting.Kubernetes.Cluster
{
    public class NetpackKubeClusterResource : NetpackClusterResource
    {
        public NetpackKubeClusterResource(string name, NetpackGatewayResource gatewayResource) : base(name, gatewayResource)
        {
        }

        public k8s.Kubernetes Kubernetes { get; private set; }
        public Kubectl Kubectl { get; private set; }
        private ReferenceExpression ConnectionString =>
           ReferenceExpression.Create(
               $"netpack:test");

        public override ReferenceExpression ConnectionStringExpression => this.TryGetLastAnnotation(out ConnectionStringRedirectAnnotation? connectionStringAnnotation)
                    ? connectionStringAnnotation.Resource.ConnectionStringExpression
                    : ConnectionString;

        public override ClusterType ClusterType => ClusterType.Kube;

        protected override async Task Configuration(ResourceState state, CancellationToken cancellationToken = default)
        {
            string kubeconfigPath = Provisioner.Properies[ClusterProperties.KubeconfigUrl];
            KubernetesClientConfiguration clientConfiguration = KubernetesClientConfiguration
                .BuildConfigFromConfigFile(kubeconfigPath);

            Kubernetes = new k8s.Kubernetes(clientConfiguration);
            Kubectl = await Kubectl.DownloadIfNotExists(state.Logger, state.ProcessStorage, cancellationToken);
            Kubectl.SetKubeconfig(kubeconfigPath);
        }

        protected override async Task Connect(ResourceState state, CancellationToken cancellationToken = default)
        {
            _ = await Kubernetes.CoreV1.ListNamespaceAsync();
        }
    }
}
