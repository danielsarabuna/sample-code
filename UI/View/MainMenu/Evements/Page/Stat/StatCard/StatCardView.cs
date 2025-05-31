using Cysharp.Threading.Tasks;
using R3;
using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;
using ViewModel;

namespace View
{
    public class StatCardView : UIElement<StatCardViewModel>
    {
        private readonly CompositeDisposable _compositeDisposable = new();

        private readonly Button UpgradeButton;
        [QueryUI] private Label LevelLabel { get; set; }
        [QueryUI] private Label UpgradeValue { get; set; }
        [QueryUI] private Label UpgradeCostValue { get; set; }
        [QueryUI] private Label NameLabel { get; set; }
        [QueryUI] private VisualElement MaxLevelRoot { get; set; }
        [QueryUI] private VisualElement UpgradeCostRoot { get; set; }

        public StatCardView(VisualElement visualElement) : base(visualElement)
        {
            UpgradeButton = visualElement.Q<Button>();
        }

        public override UniTask Initialize(StatCardViewModel viewModel)
        {
            viewModel.Name.Subscribe(x => NameLabel.text = x).AddTo(_compositeDisposable);
            viewModel.LevelLabel.Where(x => !viewModel.IsMaxLevel.CurrentValue)
                .Subscribe(x => LevelLabel.text = $"Ур.{x}")
                .AddTo(_compositeDisposable);
            viewModel.UpgradeValueLabel.Subscribe(x => UpgradeValue.text = $"+{x}").AddTo(_compositeDisposable);
            viewModel.UpgradeCostLabel.Subscribe(x => UpgradeCostValue.text = $"{x}").AddTo(_compositeDisposable);
            viewModel.IsMaxLevel.Where(x => x)
                .Subscribe(x =>
                {
                    MaxLevelRoot.style.display = DisplayStyle.Flex;
                    UpgradeCostRoot.style.display = DisplayStyle.None;
                    VisualElement.SetEnabled(false);
                    LevelLabel.text = "Макс.";
                })
                .AddTo(_compositeDisposable);

            UpgradeButton.OnClickAsObservable()
                .Subscribe(x => viewModel.UpgradeStat())
                .AddTo(_compositeDisposable);
            return UniTask.CompletedTask;
        }
    }
}