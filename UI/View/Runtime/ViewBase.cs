using System;
using Cysharp.Threading.Tasks;
using R3;
using UI.View;
using UI.ViewModel;
using UnityEngine.UIElements;
using VContainer;

namespace View
{
    public abstract class ViewBase<TViewModel> : View<TViewModel>, IDisposable
        where TViewModel : IViewModel
    {
        protected readonly CompositeDisposable CompositeDisposable = new();
        [Inject] protected EasePresets Presets { get; private set; }

        public virtual void Dispose()
        {
            CompositeDisposable?.Dispose();
        }

        protected override UniTask Show()
        {
            VisualElement.style.display = DisplayStyle.Flex;
            return base.Show();
        }

        protected override UniTask Hide()
        {
            VisualElement.style.display = DisplayStyle.None;
            VisualElement.RemoveFromHierarchy();
            return base.Hide();
        }

        protected async void HandleClose(bool value)
        {
            if (!value) return;
            await Hide();
        }

        protected async void HandleClose()
        {
            await ViewModel.Close();
        }
    }

    internal static class UIElementsExtensions
    {
        public static Observable<Unit> OnClickAsObservable(this Button button)
        {
            return Observable.Create<Unit>(observer =>
            {
                EventCallback<ClickEvent> callback = evt => observer.OnNext(Unit.Default);

                button.RegisterCallback(callback);

                return Disposable.Create(() => button.UnregisterCallback(callback));
            });
        }
    }
}