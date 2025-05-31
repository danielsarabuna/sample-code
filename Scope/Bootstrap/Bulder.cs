using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using Cysharp.Threading.Tasks;
using Model;
using UI;
using VContainer;
using VContainer.Unity;

public partial class Bootstrap
{
    private class Builder
    {
        private readonly LifetimeScope _scope;
        private readonly List<IConfigurator> _configurators;

        public Builder(LifetimeScope scope)
        {
            _scope = scope;

            _configurators = new List<IConfigurator>
            {
                new UIConfigurator(),
                new ControllerIConfigurator(),
                new ServeIConfigurator(),
                new ContentIConfigurator(),
                new UserContextIConfigurator(),
            };
        }

        public async UniTask InitializeAsync(CancellationToken cancellation)
        {
            foreach (var configurator in _configurators)
                if (configurator is IAsyncInitializer configuratorInitAsync)
                    await configuratorInitAsync.InitializeAsync(cancellation);

            _scope.Build();
        }

        public void Configure(IContainerBuilder builder)
        {
            builder.Register<ApplicationManager>(Lifetime.Singleton)
                .As<ApplicationManager>()
                .As<IStateChangeable>();
            
            foreach (var configurator in _configurators)
                configurator.Configure(builder);
        }

        public async UniTask ExecuteScheduledTasksAsync(Func<IList<LoadingState>, UniTask> taskScheduler,
            CancellationToken cancellation)
        {
            foreach (var configurator in _configurators)
                if (configurator is ITaskScheduler configuratorStartAsync)
                    await taskScheduler.Invoke(configuratorStartAsync.CreateTaskList(_scope.Container, cancellation));
        }
    }
}