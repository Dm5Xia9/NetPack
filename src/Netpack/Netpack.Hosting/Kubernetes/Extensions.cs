using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Netpack.Hosting.Gateways;
using Netpack.Hosting.Kubernetes.Cluster;
using Netpack.Hosting.Kubernetes.Forwards;
using Netpack.Hosting.ResourcePacks;

namespace Netpack.Hosting.Kubernetes
{
    public static class Extensions
    {
        public static IResourceBuilder<NetpackKubeClusterResource> AddKubeCluster(this IResourceBuilder<NetpackGatewayResource> builder, string name)
        {
            NetpackKubeClusterResource cluster = new(name, builder.Resource);
            return builder.ApplicationBuilder.AddResource(cluster)
                .WithInitialState(new CustomResourceSnapshot
                {
                    ResourceType = "Kubernetes",
                    Properties = []
                });
        }

        public static IResourceBuilder<NetpackKubeClusterForwardResource> AddForward
            (this IResourceBuilder<NetpackKubeClusterResource> builder, string name, int? port = null, string @namespace = "default")
        {
            string partName = string.Join('/', new string[] { @namespace, name }.Distinct());
            NetpackKubeClusterForwardResource forward = new(partName, builder.Resource);
            IResourceBuilder<NetpackKubeClusterForwardResource> forwardBuilder = builder.ApplicationBuilder.AddResource(forward)
                .WithInitialState(new CustomResourceSnapshot
                {
                    ResourceType = $"dev/forward",
                    Properties = []
                });

            ForwardDeploymentAnnotation deploymentAnnotation = new()
            {
                Name = name,
                Namespace = @namespace,
                Port = port,
            };
            _ = forwardBuilder.WithAnnotation(deploymentAnnotation);
            return forwardBuilder;
        }

        public static IResourceBuilder<NetpackKubeClusterForwardResource> WithResourcePack
            (this IResourceBuilder<NetpackKubeClusterForwardResource> builder, IResourcePack resourcePack, string tag = "default")
        {
            if (!builder.Resource.TryGetLastAnnotation(out ForwardDeploymentAnnotation? deployment))
            {
                return builder;
            }

            ForwardDeploymentAnnotation packDeployment = resourcePack.GetForwardDeployment(deployment.Name, tag);
            deployment.Namespace = packDeployment.Namespace;
            deployment.Port = packDeployment.Port;
            return builder;
        }


    }
}
