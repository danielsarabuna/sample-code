using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Model;
using Router;
using UI.Router;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class Bootstrap : LifetimeScope
{
    private Builder _builder;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_ANDROID || UNITY_IOS
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.multiTouchEnabled = false;
#endif
    }

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        _builder?.Configure(builder);
    }

    protected async void Start()
    {
        var token = CancellationToken.None;
        await Initialize(token);
    }

    private async UniTask Initialize(CancellationToken cancellation)
    {
        _builder = new Builder(this);
        await _builder.InitializeAsync(cancellation);
        DontDestroyOnLoad(gameObject);
        await _builder.ExecuteScheduledTasksAsync(ProcessLoadingStates, cancellation);
    }

    private async UniTask ProcessLoadingStates(IList<LoadingState> loadingStates)
    {
        IRouter<LoadingModel> router = Container.Resolve<LoadingRouter>();
        router.SetModel(new LoadingModel(loadingStates, 1));
        await router.Bind();
    }
}