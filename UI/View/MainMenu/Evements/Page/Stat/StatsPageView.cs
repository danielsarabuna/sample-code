using System;
using Cysharp.Threading.Tasks;
using R3;
using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;
using ViewModel;

namespace View
{
    public class StatsPageView : UIElement<MainMenuViewModel>, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private MainMenuViewModel _viewModel;
        [QueryUI] public StatCardView HealthCard { get; private set; }
        [QueryUI] public StatCardView AttackCard { get; private set; }
        [QueryUI] public StatCardView DefenseCard { get; private set; }
        [QueryUI] public ProgressBar LevelProgress { get; private set; }

        public StatsPageView(VisualElement visualElement) : base(visualElement)
        {
        }

        public override async UniTask Initialize(MainMenuViewModel viewModel)
        {
            _viewModel = viewModel;
            
            await HealthCard.Initialize(viewModel.HealthCard);
            await AttackCard.Initialize(viewModel.AttackCard);
            await DefenseCard.Initialize(viewModel.DefenseCard);
            
            viewModel.LevelAmount.Subscribe(x => LevelProgress.value = Convert.ToSingle(x))
                .AddTo(_compositeDisposable);
            LevelProgress.highValue = _viewModel.HealthCard.MaxLevel + _viewModel.AttackCard.MaxLevel +
                                      _viewModel.DefenseCard.MaxLevel;
            viewModel.HealthCard.LevelLabel.Subscribe(UpdateLevelLabel).AddTo(_compositeDisposable);
            viewModel.AttackCard.LevelLabel.Subscribe(UpdateLevelLabel).AddTo(_compositeDisposable);
            viewModel.DefenseCard.LevelLabel.Subscribe(UpdateLevelLabel).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        private void UpdateLevelLabel(uint newValue)
        {
            _viewModel.LevelAmount.Value = _viewModel.HealthCard.LevelLabel.CurrentValue +
                                           _viewModel.AttackCard.LevelLabel.CurrentValue +
                                           _viewModel.DefenseCard.LevelLabel.CurrentValue;
        }
    }
}