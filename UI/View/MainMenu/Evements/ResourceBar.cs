using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;
using Utilities;

namespace View
{
    public class ResourceBar : UIElement
    {
        [QueryUI] private Label Energy { get; set; }
        [QueryUI] private Label Gems { get; set; }
        [QueryUI] private Label Coins { get; set; }

        public ResourceBar(VisualElement element) : base(element)
        {
        }

        public void SetEnergy(uint value)
        {
            Energy.text = LargeNumberFormatter.Convert(value);
        }

        public void SetGems(uint value)
        {
            Gems.text = LargeNumberFormatter.Convert(value);
        }

        public void SetCoins(uint value)
        {
            Coins.text = LargeNumberFormatter.Convert(value);
        }
    }
}