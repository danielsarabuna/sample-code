using System.Collections.Generic;
using System.Collections.Specialized;
using Cysharp.Threading.Tasks;
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
    public class PopupEquipmentInfoView : ViewBase<PopupEquipmentInfoViewModel>
    {
        protected override string Key => "PopupEquipmentInfoView";
        [Inject] private IAssetManagement _assetManagement;
        [Inject] private IElementFactory _elementFactory;
        private VisualTreeAsset _propertyOrigin;
        private VisualTreeAsset _abilityOrigin;
        private VisualTreeAsset _effectOrigin;
        [QueryUI] private Button Close { get; set; }
        [QueryUI] private Button Upgrade { get; set; }
        [QueryUI] private Button Equip { get; set; }
        [QueryUI] private Button TakeOff { get; set; }
        [QueryUI] private VisualElement Popup { get; set; }
        [QueryUI] private EquipmentView EquipmentView { get; set; }
        [QueryUI] private VisualElement UpgradeIcon { get; set; }
        [QueryUI] private Label Name { get; set; }
        [QueryUI] private Label Description { get; set; }
        [QueryUI] private Label UpgradeCost { get; set; }
        [QueryUI] private VisualElement PropertyContainer { get; set; }
        [QueryUI("scroll-view")] private ScrollView ScrollView { get; set; }
        private VisualElement ContentContainer { get; set; }

        protected override async UniTask Register()
        {
            await base.Register();
            ContentContainer = ScrollView.Q<VisualElement>("unity-content-container");
            ContentContainer.Clear();
            PropertyContainer.Clear();
            ContentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);

            _propertyOrigin = await _assetManagement.LoadAssetAsync<VisualTreeAsset>("EquipmentView");
            _abilityOrigin = await _assetManagement.LoadAssetAsync<VisualTreeAsset>("AbilityView");
            _effectOrigin = await _assetManagement.LoadAssetAsync<VisualTreeAsset>("EffectView");

            ViewModel.CloseCommand.Subscribe(HandleClose).AddTo(CompositeDisposable);

            Close.OnClickAsObservable().Subscribe(x => HandleClose()).AddTo(CompositeDisposable);
            Upgrade.OnClickAsObservable().Subscribe(x => ViewModel.Upgrade()).AddTo(CompositeDisposable);
            Equip.OnClickAsObservable().Subscribe(x => ViewModel.Equip()).AddTo(CompositeDisposable);
            TakeOff.OnClickAsObservable().Subscribe(x => ViewModel.TakeOff()).AddTo(CompositeDisposable);

            ViewModel.NameLabel.Subscribe(x => Name.text = x).AddTo(CompositeDisposable);
            ViewModel.DescriptionLabel.Subscribe(x => Description.text = x).AddTo(CompositeDisposable);
            ViewModel.Properties.CollectionChanged += HandlePropertiesChanged;
            ViewModel.Ability.CollectionChanged += HandleAbilityChanged;
            ViewModel.Effects.CollectionChanged += HandleEffectsChanged;
            ViewModel.UpgradeCostLabel.Subscribe(x => UpgradeCost.text = x.ToString()).AddTo(CompositeDisposable);
            ViewModel.EquipButonActive.Subscribe(x => Equip.style.display = x ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(CompositeDisposable);
            ViewModel.TakeOffButtonActive.Subscribe(x => TakeOff.style.display = x ? DisplayStyle.Flex : DisplayStyle.None)
                .AddTo(CompositeDisposable);


            UpgradeIcon.AddToClassList("coins-icon");
            _propertyOrigin = await _assetManagement.LoadAssetAsync<VisualTreeAsset>("PropertyView");
            await EquipmentView.Initialize(ViewModel.Equipment);

            foreach (var pair in ViewModel.Properties)
                CreatePropertyView(pair.Key, pair.Value);

            foreach (var description in ViewModel.Ability)
                CreateAbilityView(description);

            foreach (var description in ViewModel.Effects)
                CreateEffectView(description);
        }

        protected override async UniTask Show()
        {
            Popup.transform.scale = Vector3.zero;
            await base.Show();
            VisualElement.experimental.animation.Start(0, 1F, 200, (element, f) => element.style.opacity = f);
            await UniTask.Delay(200);
            Popup.experimental.animation.Scale(1F, 150).Ease(Presets.Get("outCubic"));
        }

        protected override async UniTask Hide()
        {
            Popup.experimental.animation.Scale(0F, 150).Ease(Presets.Get("outCubic"));
            await UniTask.Delay(100);
            VisualElement.experimental.animation.Start(1, 0F, 100, (element, f) => element.style.opacity = f);
            await UniTask.Delay(100);
            await base.Hide();
        }

        private void HandlePropertiesChanged(
            in NotifyCollectionChangedEventArgs<KeyValuePair<string, double>> eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Add)
                CreatePropertyView(eventArgs.NewItem.Key, eventArgs.NewItem.Value);
        }

        private void HandleAbilityChanged(in NotifyCollectionChangedEventArgs<string> eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Add)
                CreateAbilityView(eventArgs.NewItem);
        }

        private void HandleEffectsChanged(in NotifyCollectionChangedEventArgs<string> eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Add)
                CreateEffectView(eventArgs.NewItem);
        }

        private async void CreatePropertyView(string name, double value)
        {
            var element = _elementFactory.CreateElement<PropertyView>(_propertyOrigin);
            await element.Initialize(name, value);
            if (element is IVisualElementContainer container) PropertyContainer.Add(container.VisualElement);
        }

        private async void CreateAbilityView(string abilityDescription)
        {
            var element = _elementFactory.CreateElement<AbilityView>(_abilityOrigin);
            await element.Initialize(abilityDescription);
            if (element is IVisualElementContainer container) ContentContainer.Add(container.VisualElement);
        }

        private async void CreateEffectView(string effectDescription)
        {
            var element = _elementFactory.CreateElement<EffectView>(_effectOrigin);
            await element.Initialize(effectDescription);
            if (element is IVisualElementContainer container) ContentContainer.Add(container.VisualElement);
        }
    }
}