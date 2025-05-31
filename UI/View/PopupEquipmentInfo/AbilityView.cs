using Cysharp.Threading.Tasks;
using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;

namespace View
{
    public class AbilityView : UIElement<string>
    {
        [QueryUI] private VisualElement Icon { get; set; }
        [QueryUI] private Label Description { get; set; }

        public AbilityView(VisualElement visualElement) : base(visualElement)
        {
        }

        public override UniTask Initialize(string description)
        {
            Icon.AddToClassList("ability-icon");
            Description.text = description;
            return UniTask.CompletedTask;
        }
    }

    public class EffectView : UIElement<string>
    {
        [QueryUI] private VisualElement Icon { get; set; }
        [QueryUI] private Label Description { get; set; }

        public EffectView(VisualElement visualElement) : base(visualElement)
        {
        }

        public override UniTask Initialize(string description)
        {
            Icon.AddToClassList("effect-icon");
            Description.text = description;
            return UniTask.CompletedTask;
        }
    }
}