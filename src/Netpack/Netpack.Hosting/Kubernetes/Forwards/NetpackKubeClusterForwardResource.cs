using Aspire.Hosting.ApplicationModel;
using k8s;
using k8s.Models;
using Netpack.Hosting.Kubernetes.Cluster;
using Netpack.Hosting.Models;
using Netpack.Hosting.Orcestraction;
using System.Collections.Immutable;

namespace Netpack.Hosting.Kubernetes.Forwards
{
    public class NetpackKubeClusterForwardResource :
        NetpackResource, INetpackResource, IResourceWithParent<NetpackKubeClusterResource>
    {
        private V1Pod? _pod;
        private int? _port;
        public NetpackKubeClusterForwardResource(string name, NetpackKubeClusterResource cluster) : base(name)
        {
            Parent = cluster;
        }

        public NetpackKubeClusterResource Parent { get; }


        public ForwardDeploymentAnnotation DeploymentAnnotation { get; private set; }

        public override Task AfterEndpointsAllocatedAsync(ResourceState state, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override async Task AfterResourcesCreatedAsync(ResourceState state, CancellationToken cancellationToken = default)
        {
            await state.WaitForResourceAsync(Parent.Name, cancellationToken, KnownResourceStates.Running);
            await state.SetState(KnownResourceStateStyles.Info, "Подготовка");
            if (!this.TryGetLastAnnotation(out ForwardDeploymentAnnotation? deploymentAnnotation))
            {
                throw new Exception();
            }

            DeploymentAnnotation = deploymentAnnotation;
            V1Deployment deployment = await Parent.Kubernetes
                .ReadNamespacedDeploymentAsync(
                deploymentAnnotation.Name,
                deploymentAnnotation.Namespace);

            V1PodList pods = await Parent.Kubernetes
                .ListNamespacedPodAsync(deploymentAnnotation.Namespace);

            V1Pod? firstPod = null;
            foreach (V1Pod? pod in pods.Items)
            {
                if (deployment.Spec.Selector.MatchLabels.All(p => pod.Metadata.Labels[p.Key] == p.Value))
                {
                    firstPod = pod;
                    break;
                }
            }

            if (firstPod == null)
            {
                throw new Exception();
            }

            _pod = firstPod;
            _port = deploymentAnnotation.Port;
            await state.SetState(KnownResourceStateStyles.Info, "Подключение");
            _ = Task.Run(async () => await Listen(state, cancellationToken));
        }

        public override Task BeforeStartAsync(ResourceState state, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }


        public async Task Listen(ResourceState state, CancellationToken cancellationToken)
        {
            await state.Try(async () =>
            {
                V1Container firstContainer = _pod.Spec.Containers[0];
                await state.SetState(KnownResourceStateStyles.Success, KnownResourceStates.Running);
                await state.StateFactory(p => p with
                {
                    Urls =
                    [
                        new UrlSnapshot("forward", $"http://localhost:8888", false)
                    ],
                    Properties =
                    [
                        new("container.image", firstContainer.Image),
                    ],
                    EnvironmentVariables = firstContainer
                        .Env.Select(p => new EnvironmentVariableSnapshot(p.Name, p.Value, true))
                        .ToImmutableArray(),
                    Commands = [
                        //new ResourceCommandSnapshot("testest", ResourceCommandState.Enabled, "d", "УРАААА", "FEEF", "fiehoiuehfe", "RecordStopRegular", IconVariant.Regular, true)
                        ],
                    Volumes = [
                        new VolumeSnapshot("ff", "fff", "volume", true)
                        ]
                });

                await Parent.Kubectl.ForwardPort(_pod.Name(), _pod.Namespace(), _port.Value, 8888);

                throw new Exception("Ошибка подключения");
            });
        }
    }
}
