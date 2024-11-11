using Aspire.Hosting.ApplicationModel;
using Netpack.Hosting.Orcestraction;
using System.Diagnostics;

namespace Netpack.Hosting.Models
{
    /// <summary>
    /// Represents an abstract resource that can be used by an application, that implements <see cref="IResource"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerToString(),nq}")]
    public abstract class NetpackResource : IResource, INetpackResource
    {
        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        public virtual string Name { get; }

        /// <summary>
        /// Gets the annotations associated with the resource.
        /// </summary>
        public virtual ResourceAnnotationCollection Annotations { get; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        protected NetpackResource(string name)
        {
            //ModelName.ValidateName(nameof(Resource), name);

            Name = name;
        }

        private string DebuggerToString()
        {
            return $@"Type = {GetType().Name}, Name = ""{Name}"", Annotations = {Annotations.Count}";
        }

        public abstract Task BeforeStartAsync(ResourceState state, CancellationToken cancellationToken = default);
        public abstract Task AfterEndpointsAllocatedAsync(ResourceState state, CancellationToken cancellationToken = default);
        public abstract Task AfterResourcesCreatedAsync(ResourceState state, CancellationToken cancellationToken = default);
    }
}
