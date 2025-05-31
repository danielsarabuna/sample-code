using Cysharp.Threading.Tasks;
using R3;
using UI;
using UnityEngine.UIElements;
using UI.Attribute;
using UnityEngine;
using ViewModel;

namespace View
{
    public class GameView : ViewBase<GameViewModel>
    {
        protected override string Key => "GameView";
        protected override UIParams UIParams => new(new Color(0.2196078F, 0.1803922F, 0.2352941F, 1F), 0.7F);
        [QueryUI] private ResourceBar ResourceBar { get; set; }
        [QueryUI("level-amount")] private Label Level { get; set; }
        [QueryUI("health-amount")] private Label Health { get; set; }
        [QueryUI("attack-amount")] private Label Attack { get; set; }
        [QueryUI("defense-amount")] private Label Defense { get; set; }
        [QueryUI("battle-speed-button")] private Button BattleSpeed { get; set; }
        [QueryUI("battle-speed-label")] private Label BattleSpeedLabel { get; set; }
        [QueryUI] public Button Leave { get; private set; }
        [QueryUI] public BottomMenuView BottomMenuView { get; private set; }

        protected override async UniTask Register()
        {
            await base.Register();

            ViewModel.CloseCommand.Subscribe(HandleClose).AddTo(CompositeDisposable);

            await BottomMenuView.Initialize(ViewModel.StoryViewModel);

            Leave.OnClickAsObservable().Subscribe(x => HandleClose()).AddTo(CompositeDisposable);
            BattleSpeed.OnClickAsObservable().Subscribe(x => ViewModel.BattleSpeedUp()).AddTo(CompositeDisposable);
            Leave.Focus();
            SetupResourceSubscriptions();
            SetupStatSubscriptions();
        }

        private void SetupResourceSubscriptions()
        {
            ViewModel.EnergyAmount.Subscribe(x => ResourceBar.SetEnergy(x)).AddTo(CompositeDisposable);
            ViewModel.GemsAmount.Subscribe(x => ResourceBar.SetGems(x)).AddTo(CompositeDisposable);
            ViewModel.CoinsAmount.Subscribe(x => ResourceBar.SetCoins(x)).AddTo(CompositeDisposable);
        }

        private void SetupStatSubscriptions()
        {
            ViewModel.Level.Subscribe(x => Health.text = $"Ур.{x}").AddTo(CompositeDisposable);
            ViewModel.HealthAmount.Subscribe(x => Health.text = $"{x}").AddTo(CompositeDisposable);
            ViewModel.AttackAmount.Subscribe(x => Attack.text = $"{x}").AddTo(CompositeDisposable);
            ViewModel.DefenseAmount.Subscribe(x => Defense.text = $"{x}").AddTo(CompositeDisposable);
            ViewModel.BattleSpeedLabel.Subscribe(x => BattleSpeedLabel.text = x).AddTo(CompositeDisposable);
        }
    }
}