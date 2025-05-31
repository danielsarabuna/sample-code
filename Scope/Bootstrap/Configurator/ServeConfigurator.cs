using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Service;
using VContainer;
using AssetManager = Service.AssetManagement;

public partial class Bootstrap
{
    private class ServeIConfigurator : IConfigurator, ITaskScheduler
    {
        void IConfigurator.Configure(IContainerBuilder builder)
        {
            builder.Register<IDebugService, DebugService>(Lifetime.Singleton);
            builder.Register<ISaveManagement, SaveManagement>(Lifetime.Singleton);
            builder.Register<IAssetManagement, AssetManager>(Lifetime.Singleton);
            builder.Register<IAudioManagement, AudioManagement>(Lifetime.Singleton);
            builder.Register<ISceneManagement, SceneManagement>(Lifetime.Singleton);
            builder.Register<IAnalytic, Analytic>(Lifetime.Singleton);
            builder.Register<ILocalization, Localization>(Lifetime.Singleton);

            builder.Register<ISyncStrategy, DefaultSyncStrategy>(Lifetime.Singleton);
            builder.Register<IContextCompressor, ContextCompressor>(Lifetime.Singleton);
            builder.Register<ProgressionService>(Lifetime.Singleton); // TODO
        }

        IList<LoadingState> ITaskScheduler.CreateTaskList(IObjectResolver builder, CancellationToken cancellation) =>
            new List<LoadingState>
            {
                new("IDebugService", () => InitializeService(builder.Resolve<IDebugService>(), cancellation)),
                new("ISaveManagement", () => InitializeService(builder.Resolve<ISaveManagement>(), cancellation)),
                new("IAssetManagement", () => InitializeService(builder.Resolve<IAssetManagement>(), cancellation)),
                new("IAudioManagement", () => InitializeService(builder.Resolve<IAudioManagement>(), cancellation)),
                new("ISceneManagement", () => InitializeService(builder.Resolve<ISceneManagement>(), cancellation)),
                new("IAnalytic", () => InitializeService(builder.Resolve<IAnalytic>(), cancellation)),
                new("ILocalization", () => InitializeService(builder.Resolve<ILocalization>(), cancellation)),
            };

        private async UniTask InitializeService(object o, CancellationToken cancellation)
        {
            if (o is IService service) await service.InitializeAsync(cancellation);
        }
    }
}