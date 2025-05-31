using System;
using System.Collections.Generic;
using System.Linq;
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
    public class InventoryView : UIElement<InventoryViewModel>, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly Dictionary<string, EquipmentView> _equipmentViews = new();
        private InventoryViewModel _viewModel;
        private VisualTreeAsset _origin;
        [Inject] private IAssetManagement _assetManagement;
        [Inject] private IElementFactory _elementFactory;
        [QueryUI("scroll-view")] private ScrollView ScrollView { get; set; }
        [QueryUI] private Button AutoSelect { get; set; }
        [QueryUI] private Button AddCapacity { get; set; }
        [QueryUI] private Button AllSort { get; set; }
        [QueryUI] private Button WeaponSort { get; set; }
        [QueryUI] private Button ArmorSort { get; set; }
        [QueryUI] private Button AccessorySort { get; set; }
        [QueryUI] private Label Capacity { get; set; }
        private VisualElement ContentContainer { get; set; }

        public InventoryView(VisualElement visualElement) : base(visualElement)
        {
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            _viewModel?.Dispose();
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            AutoSelect.OnClickAsObservable()
                .Subscribe(x => _viewModel.HandleAutoSelect())
                .AddTo(_compositeDisposable);

            AddCapacity.OnClickAsObservable()
                .Subscribe(x => _viewModel.HandleAddCapacity())
                .AddTo(_compositeDisposable);

            AllSort.OnClickAsObservable()
                .Subscribe(x =>
                {
                    ScrollToTop();
                    _viewModel.HandleNoSort();
                })
                .AddTo(_compositeDisposable);

            WeaponSort.OnClickAsObservable()
                .Subscribe(x =>
                {
                    ScrollToTop();
                    _viewModel.HandleWeaponSort();
                })
                .AddTo(_compositeDisposable);

            ArmorSort.OnClickAsObservable()
                .Subscribe(x =>
                {
                    ScrollToTop();
                    _viewModel.HandleArmorSort();
                })
                .AddTo(_compositeDisposable);

            AccessorySort.OnClickAsObservable()
                .Subscribe(x =>
                {
                    ScrollToTop();
                    _viewModel.HandleAccessorySort();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void Initialize()
        {
            base.Initialize();
            ContentContainer = ScrollView.Q<VisualElement>("unity-content-container");
            ContentContainer.Clear();
        }

        public override async UniTask Initialize(InventoryViewModel viewModel)
        {
            _origin = await _assetManagement.LoadAssetAsync<VisualTreeAsset>("EquipmentView");
            _viewModel = viewModel;
            _viewModel.Capacity.Subscribe(x => UpdateCapacityDisplay()).AddTo(_compositeDisposable);
            _viewModel.Items.ObserveAdd()
                .Subscribe(async x =>
                {
                    var equipmentView = await Create(_viewModel, x.Value);
                    _equipmentViews.Add(x.Value.Name, equipmentView);
                    UpdateCapacityDisplay();
                })
                .AddTo(_compositeDisposable);

            _viewModel.Items.ObserveRemove()
                .Subscribe(removeEvent =>
                {
                    if (!_equipmentViews.TryGetValue(removeEvent.Value.Name, out var view)) return;
                    view.Dispose();
                    _equipmentViews.Remove(removeEvent.Value.Name);
                    UpdateCapacityDisplay();
                })
                .AddTo(_compositeDisposable);

            await CreateInitialEquipmentViews();

            _viewModel.SortItems.ObserveAdd()
                .Subscribe(x =>
                {
                    if (!_equipmentViews.TryGetValue(x.Value.Name, out var view)) return;
                    view.Show();
                })
                .AddTo(_compositeDisposable);

            _viewModel.SortItems.ObserveRemove()
                .Subscribe(x =>
                {
                    if (!_equipmentViews.TryGetValue(x.Value.Name, out var view)) return;
                    view.Hide();
                })
                .AddTo(_compositeDisposable);
        }

        private void UpdateCapacityDisplay()
        {
            Capacity.text = $"{_viewModel.Items.Count}/{_viewModel.Capacity}";
        }

        private void ScrollToTop()
        {
            ScrollView.scrollOffset = new Vector2(ScrollView.scrollOffset.x, 0);
        }

        private async UniTask CreateInitialEquipmentViews()
        {
            var tasks = _viewModel.Items.Select(async equipmentModel =>
            {
                var equipmentView = await Create(_viewModel, equipmentModel);
                _equipmentViews.Add(equipmentModel.Name, equipmentView);
            }).ToList();

            await UniTask.WhenAll(tasks);
        }

        private async UniTask<EquipmentView> Create(InventoryViewModel viewModel, EquipmentModel model)
        {
            var element = _elementFactory.CreateElement<EquipmentView>(_origin);
            await element.Initialize(model, viewModel);
            if (element is IVisualElementContainer container) ContentContainer.Add(container.VisualElement);
            return element;
        }
    }
}