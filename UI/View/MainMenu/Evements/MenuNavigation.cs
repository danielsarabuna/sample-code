using R3;
using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;

namespace View
{
    public class MenuNavigation : UIElement
    {
        public readonly ReactiveCommand<string> OnClick = new();
        [QueryUI] public Button ShopButton { get; private set; }
        [QueryUI] public Button EquipButton { get; private set; }
        [QueryUI] public Button GameButton { get; private set; }
        [QueryUI] public Button StatsButton { get; private set; }
        [QueryUI("pvp-button")] public Button PvPButton { get; private set; }

        public MenuNavigation(VisualElement element) : base(element)
        {
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();
            ShopButton.RegisterCallback<ClickEvent>(_ => OnClick.Execute("Shop"));
            EquipButton.RegisterCallback<ClickEvent>(_ => OnClick.Execute("Equip"));
            GameButton.RegisterCallback<ClickEvent>(_ => OnClick.Execute("Game"));
            StatsButton.RegisterCallback<ClickEvent>(_ => OnClick.Execute("Stats"));
            PvPButton.RegisterCallback<ClickEvent>(_ => OnClick.Execute("PvP"));
        }
    }
}