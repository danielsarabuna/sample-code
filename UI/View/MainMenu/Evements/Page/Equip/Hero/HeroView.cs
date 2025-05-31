using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Cysharp.Threading.Tasks;
using Model;
using NUnit.Framework;
using ObservableCollections;
using R3;
using Service;
using UI.Attribute;
using UI.View;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using ViewModel;

namespace View
{
    public class HeroView : UIElement<HeroViewModel>, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly Dictionary<EquipmentSlot, VisualElement> _equipmentIcons = new();
        private HeroViewModel _viewModel;
        [QueryUI("hero-name-value")] private Label HeroName { get; set; }
        [QueryUI("level-amount")] private Label Level { get; set; }
        [QueryUI("experience-amount")] private Label Experience { get; set; }
        [QueryUI("experience-progress-bar")] private VisualElement ExperienceProgressBar { get; set; }
        [QueryUI("health-amount")] private Label Health { get; set; }
        [QueryUI("attack-amount")] private Label Attack { get; set; }
        [QueryUI("defense-amount")] private Label Defense { get; set; }
        [Inject] private readonly ProgressionService _progressionService;
        [Inject] private readonly IAssetManagement _assetManagement;
        [Inject] private readonly PreviewHeroEquipmentView _previewHeroEquipmentView;

        public HeroView(VisualElement visualElement) : base(visualElement)
        {
        }

        public void Dispose()
        {
            _viewModel.HeroEquipment.ObservableEquipmentSlots.CollectionChanged -= HandleEquipmentSlotsChanged;
            _compositeDisposable?.Dispose();
            _viewModel?.Dispose();
        }

        public override async UniTask Initialize(HeroViewModel viewModel)
        {
            _viewModel = viewModel;

            await _previewHeroEquipmentView.Initialize(_viewModel.HeroEquipment);

            _viewModel.HeroNameValue.Subscribe(x => HeroName.text = x).AddTo(_compositeDisposable);
            _viewModel.LevelAmount.Subscribe(x => Level.text = $"{x}").AddTo(_compositeDisposable);
            _viewModel.ExperienceAmount.Subscribe(x =>
            {
                var calculator = _progressionService.Get<IProgressionCalculator>("experience");
                var max = calculator.GetValue(_viewModel.LevelAmount.CurrentValue);
                Experience.text = $"{x}";
                var flexGrow = 1F / max * x;
                ExperienceProgressBar.style.flexGrow = flexGrow > 0.05F ? flexGrow : 0F;
            }).AddTo(_compositeDisposable);
            _viewModel.HealthAmount.Subscribe(x => Health.text = $"{x}").AddTo(_compositeDisposable);
            _viewModel.AttackAmount.Subscribe(x => Attack.text = $"{x}").AddTo(_compositeDisposable);
            _viewModel.DefenseAmount.Subscribe(x => Defense.text = $"{x}").AddTo(_compositeDisposable);

            _viewModel.HeroEquipment.ObservableEquipmentSlots.CollectionChanged += HandleEquipmentSlotsChanged;

            foreach (var pair in _viewModel.HeroEquipment.EquipmentSlots)
            {
                var visualElement = VisualElement.Q<VisualElement>($"{pair.Key.ToString().ToLower()}-icon");
                Assert.IsNotNull(visualElement);
                _equipmentIcons[pair.Key] = visualElement;
                if (pair.Value == null) continue;
                var className = $"{pair.Key.ToString().ToLower()}-icon";
                visualElement.RemoveFromClassList(className);
                ApplyEquipmentIcon(pair.Value, visualElement);
            }
        }

        private void HandleEquipmentSlotsChanged(
            in NotifyCollectionChangedEventArgs<KeyValuePair<EquipmentSlot, EquipmentModel>> eventArgs)
        {
            if (!_equipmentIcons.TryGetValue(eventArgs.NewItem.Key, out var visualElement)) return;
            if (eventArgs.Action != NotifyCollectionChangedAction.Replace) return;
            var className = $"{eventArgs.NewItem.Key.ToString().ToLower()}-icon";
            if (eventArgs.NewItem.Value != null)
            {
                visualElement.RemoveFromClassList(className);
                ApplyEquipmentIcon(eventArgs.NewItem.Value, visualElement);
            }
            else
            {
                visualElement.AddToClassList(className);
                var element = visualElement.Q("slot");
                element.style.display = DisplayStyle.None;
            }
        }

        private async void ApplyEquipmentIcon(EquipmentModel equipmentModel, VisualElement visualElement)
        {
            var sprite = await _assetManagement.LoadAssetAsync<Sprite>(equipmentModel.Icon);
            var element = visualElement.Q("slot");
            element.style.backgroundImage = new StyleBackground(sprite);
            element.style.display = DisplayStyle.Flex;
        }
    }
}