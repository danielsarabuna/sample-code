using System;
using System.Collections.Specialized;
using Cysharp.Threading.Tasks;
using Model;
using ObservableCollections;
using R3;
using Service;
using UI;
using UI.Attribute;
using UI.View;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using ViewModel;

namespace View
{
    public class BottomMenuView : UIElement<StoryViewModel>, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private StoryViewModel _viewModel;
        private VisualTreeAsset _origin;
        [Inject] private IAssetManagement _assetManagement;
        [Inject] private IElementFactory _elementFactory;
        [QueryUI] private Button Move { get; set; }
        [QueryUI("choice-1")] private Button Choice1 { get; set; }
        [QueryUI("choice-2")] private Button Choice2 { get; set; }
        [QueryUI("scroll-view")] private ScrollView ScrollView { get; set; }
        private VisualElement ContentContainer { get; set; }

        public BottomMenuView(VisualElement visualElement) : base(visualElement)
        {
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            _viewModel.Challenges.CollectionChanged -= HandleChallengesChanged;
        }

        protected override void Initialize()
        {
            base.Initialize();
            ContentContainer = ScrollView.Q<VisualElement>("unity-content-container");
            ContentContainer.Clear();
        }

        public override async UniTask Initialize(StoryViewModel viewModel)
        {
            _viewModel = viewModel;
            _origin = await _assetManagement.LoadAssetAsync<VisualTreeAsset>("ChallengeView");
            _viewModel.IsMoveActive
                .Subscribe(x => Move.style.display  = x ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(_compositeDisposable);
            
            _viewModel.IsChoice1Active
                .Subscribe(x => Choice1.style.display  = x ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(_compositeDisposable);
            
            _viewModel.IsChoice2Active
                .Subscribe(x => Choice2.style.display  = x ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(_compositeDisposable);
            
            _viewModel.Choice1Label
                .Subscribe(x => Choice1.text  = x)
                .AddTo(_compositeDisposable);
            
            _viewModel.Choice2Label
                .Subscribe(x => Choice2.text  = x)
                .AddTo(_compositeDisposable);

            _viewModel.Challenges.CollectionChanged += HandleChallengesChanged;
        }


        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();
            Move.OnClickAsObservable()
                .Subscribe(x => _viewModel.Continue())
                .AddTo(_compositeDisposable);
            
            Choice1.OnClickAsObservable()
                .Subscribe(x => _viewModel.ChooseChoice(0))
                .AddTo(_compositeDisposable);
            
            Choice2.OnClickAsObservable()
                .Subscribe(x => _viewModel.ChooseChoice(1))
                .AddTo(_compositeDisposable);
        }

        private void ScrollToBottom()
        {
            ScrollView.scrollOffset = new Vector2(ScrollView.scrollOffset.x, float.MaxValue);
        }

        private void HandleChallengesChanged(in NotifyCollectionChangedEventArgs<ChallengeModel> eventArgs)
        {
            if (eventArgs.Action != NotifyCollectionChangedAction.Add) return;
            AddChallengeView(eventArgs.NewItem, _viewModel);
        }

        private async void AddChallengeView(ChallengeModel model, StoryViewModel viewModel)
        {
            await Create(model, viewModel);
            await UniTask.Delay(50); // TODO
            ScrollToBottom();
            await UniTask.Delay(50);
            ScrollToBottom();
            await UniTask.Delay(50);
            ScrollToBottom();
        }

        private async UniTask<ChallengeView> Create(ChallengeModel model, StoryViewModel viewModel)
        {
            var element = _elementFactory.CreateElement<ChallengeView>(_origin);
            await element.Initialize(model, viewModel);
            if (element is IVisualElementContainer container) ContentContainer.Add(container.VisualElement);
            return element;
        }
    }
}