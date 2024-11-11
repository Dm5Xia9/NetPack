namespace Netpack.GatewayApi.Clusters
{
    public class ExternalClusterProvisioner : IClusterProvisioner
    {
        private readonly ExternalClusterAnnotation _annotation;
        public ExternalClusterProvisioner(ExternalClusterAnnotation externalClusterAnnotation)
        {
            _annotation = externalClusterAnnotation;
        }

        public string GetKubeconfig()
        {
            return _annotation.Kubeconfig;
        }
    }
}
