using Cysharp.Threading.Tasks;
using R3;
using UI;
using UnityEngine.UIElements;
using UI.Attribute;
using UnityEngine;
using ViewModel;

namespace View
{
    public class MainMenuView : ViewBase<MainMenuViewModel>
    {
        protected override string Key => "MainMenuView";
        protected override UIParams UIParams => new(new Color(0.2196078F, 0.1803922F, 0.2352941F, 1F), 0.7F);
        [QueryUI] private Button Start { get; set; }
        [QueryUI] private ResourceBar ResourceBar { get; set; }
        [QueryUI] private PageNavigator PageNavigator { get; set; }
        [QueryUI] private MenuNavigation MenuNavigation { get; set; }

        protected override async UniTask Register()
        {
            await base.Register();
            SetupResourceSubscriptions();
            SetupPageNavigation();
            SetupMenuNavigation();

            await PageNavigator.Initialize(ViewModel);
        }

        private void SetupResourceSubscriptions()
        {
            ViewModel.EnergyAmount.Subscribe(x => ResourceBar.SetEnergy(x)).AddTo(CompositeDisposable);
            ViewModel.GemsAmount.Subscribe(x => ResourceBar.SetGems(x)).AddTo(CompositeDisposable);
            ViewModel.CoinsAmount.Subscribe(x => ResourceBar.SetCoins(x)).AddTo(CompositeDisposable);
        }

        private void SetupPageNavigation()
        {
            ViewModel.ActivePage.Subscribe(x => PageNavigator.SetPage(x)).AddTo(CompositeDisposable);
            ViewModel.IsShopPageActiveValue.Subscribe(x => MenuNavigation.ShopButton.SetEnabled(x))
                .AddTo(CompositeDisposable);
            ViewModel.IsEquipPageActiveValue.Subscribe(x => MenuNavigation.EquipButton.SetEnabled(x))
                .AddTo(CompositeDisposable);
            ViewModel.IsGamePageActiveValue.Subscribe(x => MenuNavigation.GameButton.SetEnabled(x))
                .AddTo(CompositeDisposable);
            ViewModel.IsStatsPageActiveValue.Subscribe(x => MenuNavigation.StatsButton.SetEnabled(x))
                .AddTo(CompositeDisposable);
            ViewModel.IsPvPPageActiveValue.Subscribe(x => MenuNavigation.PvPButton.SetEnabled(x))
                .AddTo(CompositeDisposable);
        }

        private void SetupMenuNavigation()
        {
            ViewModel.CloseCommand.Subscribe(HandleClose).AddTo(CompositeDisposable);
            MenuNavigation.OnClick.Subscribe(ViewModel.ShowPage).AddTo(CompositeDisposable);
            Start.OnClickAsObservable().Subscribe(x => HandleClose()).AddTo(CompositeDisposable);
            Start.Focus();
        }
    }
}