using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer;

public interface IConfigurator
{
    void Configure(IContainerBuilder builder);
}

public interface IAsyncInitializer
{
    UniTask InitializeAsync(CancellationToken cancellation);
}

public interface ITaskScheduler
{
    IList<LoadingState> CreateTaskList(IObjectResolver builder, CancellationToken cancellation);
}

public sealed class LoadingState
{
    public readonly string Title;
    public readonly Func<UniTask> ProgressCallback;

    public LoadingState(string title, Func<UniTask> progressCallback)
    {
        Title = title;
        ProgressCallback = progressCallback;
    }
}