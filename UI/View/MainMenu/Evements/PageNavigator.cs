using Cysharp.Threading.Tasks;
using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;
using ViewModel;

namespace View
{
    public class PageNavigator : UIElement<MainMenuViewModel>
    {
        [QueryUI] private VisualElement ShopPage { get; set; }
        [QueryUI("equip-page")] private EquipPageView EquipPageView { get; set; }
        [QueryUI] private VisualElement GamePage { get; set; }
        [QueryUI("stats-page")] public StatsPageView StatsPageView { get; set; }
        [QueryUI("pvp-page")] private VisualElement PvPPage { get; set; }

        public PageNavigator(VisualElement element) : base(element)
        {
        }

        public override async UniTask Initialize(MainMenuViewModel viewModel)
        {
            await UniTask.WhenAll(StatsPageView.Initialize(viewModel),
                EquipPageView.Initialize(viewModel));
        }

        public void SetPage(string page)
        {
            ShopPage.style.display = page == "Shop" ? DisplayStyle.Flex : DisplayStyle.None;
            if (page == "Equip") EquipPageView.Show();
            else EquipPageView.Hide();
            GamePage.style.display = page == "Game" ? DisplayStyle.Flex : DisplayStyle.None;
            if (page == "Stats") StatsPageView.Show();
            else StatsPageView.Hide();
            PvPPage.style.display = page == "PvP" ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}