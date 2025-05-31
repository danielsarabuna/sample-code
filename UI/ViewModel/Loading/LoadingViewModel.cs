using System;
using Cysharp.Threading.Tasks;
using Model;
using R3;
using UnityEngine;
using UI.ViewModel;

namespace ViewModel
{
    public sealed class LoadingViewModel : ViewModelBase<LoadingModel>
    {
        public readonly ReactiveProperty<string> Title = new();
        public readonly ReactiveProperty<float> Progress = new();

        protected override async UniTask Register()
        {
            await base.Register();
        }

        public override async UniTask Open()
        {
            var collection = Model.States;
            var index = 0F;
            var length = collection.Count;
            var start = Time.time;

            foreach (var loadingState in collection)
            {
                Title.Value = loadingState.Title;
                await loadingState.ProgressCallback.Invoke();
                Progress.Value = ++index / length;
            }

            await UniTask.Delay(1000);
            
            if (start + Model.Delay > Time.time)
            {
                var millisecondsDelay = Convert.ToInt32((Model.Delay - (Time.time - start)) * 1000);
                await UniTask.Delay(millisecondsDelay);
            }
        }

        public override UniTask Close()
        {
            CloseCommand.Execute(true);
            return UniTask.CompletedTask;
        }
    }
}