using Aspire.Hosting.ApplicationModel;
using Netpack.Hosting.Gateways;
using Netpack.Hosting.Kubernetes.Forwards;

namespace Netpack.Hosting.ResourcePacks
{
    public static class Extensions
    {

        public static IResourcePack AddRemoteResourcePack
            (this IResourceBuilder<NetpackGatewayResource> builder, string name)
        {
            return new ResourcePackResource(name);
        }

        public static IResourcePack AddLocalResourcePack
            (this IResourceBuilder<NetpackGatewayResource> builder, string name = "local")
        {
            return new ResourcePackResource(name);
        }

        public static IResourcePack WithLocalResourcePack
            (this IResourcePack builder, string name)
        {
            return builder;
        }
    }

    public class ResourcePackResource : Resource, IResourcePack
    {
        public ResourcePackResource(string name) : base(name)
        {
        }

        public ForwardDeploymentAnnotation GetForwardDeployment(string name, string tag = "default")
        {
            return new ForwardDeploymentAnnotation { Name = name, Namespace = name, Port = 8080 };
        }
    }

    public interface IResourcePack : IResource
    {
        public ForwardDeploymentAnnotation GetForwardDeployment(string name, string tag = "default");
    }
}
