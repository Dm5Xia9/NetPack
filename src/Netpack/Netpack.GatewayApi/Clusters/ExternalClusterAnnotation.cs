namespace Netpack.GatewayApi.Clusters
{
    public class ExternalClusterAnnotation : IClusterAnnotation
    {
        public ExternalClusterAnnotation(string kubeconfig)
        {
            Kubeconfig = kubeconfig;
        }

        public string Kubeconfig { get; set; }
    }
}
