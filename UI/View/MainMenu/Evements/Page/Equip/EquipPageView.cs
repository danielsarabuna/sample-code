using System;
using Cysharp.Threading.Tasks;
using R3;
using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;
using ViewModel;

namespace View
{
    public class EquipPageView : UIElement<MainMenuViewModel>, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private MainMenuViewModel _viewModel;
        [QueryUI("hero-view")] private HeroView Hero { get; set; }
        [QueryUI("inventory-view")] private InventoryView Inventory { get; set; }


        public EquipPageView(VisualElement visualElement) : base(visualElement)
        {
        }

        public override async UniTask Initialize(MainMenuViewModel viewModel)
        {
            _viewModel = viewModel;
            await UniTask.WhenAll(Hero.Initialize(viewModel.HeroViewModel),
                Inventory.Initialize(viewModel.InventoryViewModel));
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
            Hero?.Dispose();
            Inventory?.Dispose();
        }
    }
}