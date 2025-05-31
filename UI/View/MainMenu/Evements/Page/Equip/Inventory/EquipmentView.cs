using System;
using Cysharp.Threading.Tasks;
using Model;
using Service;
using UI.Attribute;
using UI.View;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using ViewModel;

namespace View
{
    public class EquipmentView : UIElement<EquipmentModel, InventoryViewModel>, IDisposable
    {
        [QueryUI("background")] private VisualElement Background { get; set; }
        [QueryUI("frame")] private VisualElement Frame { get; set; }
        [QueryUI("icon")] private VisualElement Icon { get; set; }
        [QueryUI("select")] private VisualElement Select { get; set; }
        [QueryUI("count")] private Label Count { get; set; }

        [Inject] private IAssetManagement _assetManagement;
        private InventoryViewModel _viewModel;
        private EquipmentModel _model;

        public EquipmentView(VisualElement visualElement) : base(visualElement)
        {
            VisualElement.RegisterCallback<ClickEvent>(HandleClick);
        }

        public void Dispose()
        {
            _viewModel?.Dispose();
            VisualElement.RemoveFromHierarchy();
            VisualElement.UnregisterCallback<ClickEvent>(HandleClick);
        }

        public async UniTask Initialize(EquipmentModel model)
        {
            Background.AddToClassList($"{model.Rarity.ToString().ToLower()}-rarity");
            Frame.AddToClassList($"{model.Rarity.ToString().ToLower()}-frame-rarity");
            var icon = await _assetManagement.LoadAssetAsync<Sprite>(model.Icon);
            Icon.style.backgroundImage = new StyleBackground(icon);
        }

        public override UniTask Initialize(EquipmentModel model, InventoryViewModel viewModel)
        {
            _viewModel = viewModel;
            _model = model;
            return Initialize(model);
        }

        private void HandleClick(ClickEvent evt)
        {
            _viewModel?.HandleShowPopupEquipmentInfo(_model);
        }
    }
}