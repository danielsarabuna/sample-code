using Cysharp.Threading.Tasks;
using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;

namespace View
{
    public class PropertyView : UIElement<string, double>
    {
        [QueryUI] private Label Title { get; set; }
        [QueryUI] private Label Value { get; set; }

        public PropertyView(VisualElement visualElement) : base(visualElement)
        {
        }

        public override UniTask Initialize(string title, double value)
        {
            Title.text = title;
            Value.text = $"+{value:#}";
            return UniTask.CompletedTask;
        }
    }
}