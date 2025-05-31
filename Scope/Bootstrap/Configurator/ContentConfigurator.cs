using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

public partial class Bootstrap
{
    private class ContentIConfigurator : IConfigurator, IAsyncInitializer, IDisposable
    {
        private EquipmentCollection _equipments;
        private ActorCollection _actors;
        private AsyncOperationHandle<TextAsset> _asyncOperationHandle;

        async UniTask IAsyncInitializer.InitializeAsync(CancellationToken cancellation)
        {
            _asyncOperationHandle = Addressables.LoadAssetAsync<TextAsset>("equipment-collection");
            var equipmentTextAsset = await _asyncOperationHandle.Task;
            _equipments = EquipmentCollection.FromJson(equipmentTextAsset.text);
            
            _asyncOperationHandle = Addressables.LoadAssetAsync<TextAsset>("actor-collection");
            var actorTextAsset = await _asyncOperationHandle.Task;
            _actors = ActorCollection.FromJson(actorTextAsset.text);
        }

        void IConfigurator.Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_equipments);
            builder.RegisterInstance(_actors);
        }

        public void Dispose()
        {
            _asyncOperationHandle.Release();
        }
    }
}