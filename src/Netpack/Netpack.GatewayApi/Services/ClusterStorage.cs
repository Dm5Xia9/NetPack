using Netpack.GatewayApi.Clusters;

namespace Netpack.GatewayApi.Services
{
    public class ClusterStorage
    {
        public void AddExternalCluster(string name, string kubeconfig)
        {
            var cluster = new Cluster(ClusterType.Kube, new ExternalClusterAnnotation(kubeconfig))
            { Name = name };

            var clusterJson = cluster.ToJson();
            File.WriteAllText($"{name}.json", clusterJson);
        }
    }
}
