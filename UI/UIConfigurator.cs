using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Router;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using VContainer.Unity;
using View;
using ViewModel;
using Object = UnityEngine.Object;

namespace UI
{
    public class UIConfigurator : IConfigurator, IAsyncInitializer, IDisposable
    {
        private AsyncOperationHandle<GameObject> _uiManagerAsyncOperation;
        private AsyncOperationHandle<EasePresets> _easePresetsAsyncOperation;
        private IUIManager UIManager { get; set; }
        private EasePresets EasePresets { get; set; }

        async UniTask IAsyncInitializer.InitializeAsync(CancellationToken cancellation)
        {
            _uiManagerAsyncOperation = Addressables.LoadAssetAsync<GameObject>("UIManager");
            var origin = await _uiManagerAsyncOperation.Task;
            var clone = Object.Instantiate(origin);
            Object.DontDestroyOnLoad(clone);
            UIManager = clone.GetComponent<UIManager>();
            _easePresetsAsyncOperation = Addressables.LoadAssetAsync<EasePresets>("EasePresets");
            EasePresets = await _easePresetsAsyncOperation.Task;
            EasePresets.Bind();
        }

        void IConfigurator.Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(UIManager).As<IElementFactory>();
            builder.RegisterComponent(EasePresets);
            
            builder.Register<LoadingRouter>(Lifetime.Transient);
            builder.Register<LoadingViewModel>(Lifetime.Transient);
            builder.Register<LoadingView>(Lifetime.Transient);
        }

        public void Dispose()
        {
            _uiManagerAsyncOperation.Release();
            _easePresetsAsyncOperation.Release();
        }
    }
}