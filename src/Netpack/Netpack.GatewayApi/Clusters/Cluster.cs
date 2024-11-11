using Newtonsoft.Json.Linq;

namespace Netpack.GatewayApi.Clusters
{
    public class Cluster
    {
        public Cluster(ClusterType clusterType, IClusterAnnotation clusterAnnotation)
        {
            ClusterType = clusterType;
            ClusterAnnotation = clusterAnnotation;
            UpdateClusterProvisioner();
        }

        public required string Name { get; set; }
        public ClusterType ClusterType { get; private set; }
        public ClusterProvisionerDriver Driver { get; private set; }
        public IClusterAnnotation ClusterAnnotation { get; private set; }
        public IClusterProvisioner ClusterProvisioner { get; private set; }

        public void SetClusterAnnotation(IClusterAnnotation clusterProvisioner)
        {
            ClusterAnnotation = clusterProvisioner;
            UpdateClusterProvisioner();
        }

        private void UpdateClusterProvisioner()
        {
            if (ClusterAnnotation is ExternalClusterAnnotation externalCluster)
            {
                ClusterProvisioner = new ExternalClusterProvisioner(externalCluster);
                Driver = ClusterProvisionerDriver.External;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static Cluster Load(string json)
        {
            JObject jsonObject = JObject.Parse(json);
            var name = jsonObject[nameof(Name)]!.ToString();
            string clusterTypeString = jsonObject[nameof(ClusterType)].ToString();
            ClusterType clusterType = (ClusterType)Enum.Parse(typeof(ClusterType), clusterTypeString);
            var clusterAnnotation = clusterType switch
            {
                ClusterType.Kube => jsonObject[nameof(ClusterAnnotation)]!.ToObject<ExternalClusterAnnotation>()!,
                _ => throw new InvalidOperationException()
            };

            return new Cluster(clusterType, clusterAnnotation) { Name = name };
        }

        public string ToJson()
        {
            var jsonObject = new JObject
            {
                [nameof(Name)] = Name,
                [nameof(ClusterType)] = ClusterType.ToString(),
                [nameof(ClusterAnnotation)] = JToken.FromObject(ClusterAnnotation)
            };

            return jsonObject.ToString();
        }
    }
}
