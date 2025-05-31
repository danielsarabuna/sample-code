using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.UIElements;
using UI.Attribute;
using ViewModel;

namespace View
{
    public sealed class LoadingView : ViewBase<LoadingViewModel>
    {
        private const string FormatProgress = "{0}%";
        protected override string Key => "LoadingView";
        protected override int Priority => 25;

        [QueryUI] private Label Title { get; set; }
        [QueryUI] private Label ProgressLabel { get; set; }
        [QueryUI] private VisualElement ProgressValue { get; set; }

        protected override async UniTask Register()
        {
            await base.Register();
            ViewModel.Title.Subscribe(HandleTitle).AddTo(CompositeDisposable);
            ViewModel.Progress.Subscribe(HandleProgress).AddTo(CompositeDisposable);

            ViewModel.CloseCommand.Subscribe(HandleClose).AddTo(CompositeDisposable);
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
            return UniTask.CompletedTask;
        }

        private void HandleTitle(string value)
        {
            Title.text = value;
        }

        private void HandleProgress(float value)
        {
            ProgressLabel.text = string.Format(FormatProgress, Math.Round(value * 100, 2));
            ProgressValue.style.flexGrow = value;
        }
    }
}