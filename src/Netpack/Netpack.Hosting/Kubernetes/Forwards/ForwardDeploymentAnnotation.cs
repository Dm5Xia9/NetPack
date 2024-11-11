using Aspire.Hosting.ApplicationModel;

namespace Netpack.Hosting.Kubernetes.Forwards
{
    public class ForwardDeploymentAnnotation : IResourceAnnotation
    {
        public string Namespace { get; set; } = "default";
        public required string Name { get; set; }
        public int? Port { get; set; }
    }
}
