using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Logging;
using Netpack.Hosting.Models;
using Netpack.Hosting.Processes;

namespace Netpack.Hosting.Orcestraction
{
    public class ResourceState
    {
        private readonly ResourceNotificationService _notificationService;
        private readonly ResourceLoggerService _loggerService;
        private bool _firstOneIs = false;

        public ResourceState(INetpackResource resource, ResourceNotificationService notificationService, ResourceLoggerService loggerService, IProcessStorage processStorage)
        {
            Resource = resource;
            _notificationService = notificationService;
            _loggerService = loggerService;
            ProcessStorage = processStorage;
            Logger = loggerService.GetLogger(resource);
        }

        public ILogger Logger { get; }

        public bool IsError { get; private set; } = false;

        public IProcessStorage ProcessStorage { get; }

        public INetpackResource Resource { get; }

        public Task SetState(ResourceStateSnapshot resourceState)
        {
            return _notificationService.PublishUpdateAsync(Resource, p => p with
            {
                State = resourceState
            });
        }

        public Task SetState(string style, string? message = null)
        {
            return _notificationService.PublishUpdateAsync(Resource, p => p with
            {
                State = new(message ?? style, style)
            });
        }

        public Task StateFactory(Func<CustomResourceSnapshot, CustomResourceSnapshot> stateFactory)
        {
            return _notificationService.PublishUpdateAsync(Resource, stateFactory);
        }

        public async Task Try(Func<Task> action, string? message = null)
        {
            try
            {
                await action();
            }
            catch (Exception e)
            {
                if (IsError)
                {
                    throw;
                }

                IsError = true;
                await SetState(KnownResourceStateStyles.Error, message);
                Logger.LogError(new EventId(0), e, message ?? "Ошибка исполнения ресурса");

                throw;
            }
        }

        public IAsyncDisposable Scope(string? message = null)
        {
            if (_firstOneIs)
            {
                return new StateScope(this, new(message ?? KnownResourceStates.Starting, KnownResourceStateStyles.Info));
            }
            else
            {
                _firstOneIs = true;
                return new StateScope(this, new(message ?? KnownResourceStates.Running, KnownResourceStateStyles.Success));
            }
        }

        public Task WaitForResourceAsync(IResource resource, CancellationToken cancellationToken, params string[] statuses)
        {
            return _notificationService.WaitForResourceAsync(resource.Name, statuses, cancellationToken);
        }

        public Task WaitForResourceAsync(string resource, CancellationToken cancellationToken, params string[] statuses)
        {
            return _notificationService.WaitForResourceAsync(resource, statuses, cancellationToken);
        }

    }

    public class StateScope : IAsyncDisposable
    {
        private readonly ResourceState _resourceState;
        private readonly ResourceStateSnapshot _successState;

        public StateScope(ResourceState resourceState, ResourceStateSnapshot successState)
        {
            _resourceState = resourceState;
            _successState = successState;
        }

        public async ValueTask DisposeAsync()
        {
            if (_resourceState.IsError)
            {
                return;
            }

            await _resourceState.SetState(_successState);
        }
    }
}
