using Cysharp.Threading.Tasks;
using Model;
using UI.Attribute;
using UI.View;
using UnityEngine.UIElements;
using ViewModel;

namespace View
{
    public class ChallengeView : UIElement<ChallengeModel, StoryViewModel>
    {
        [QueryUI] private Label Day { get; set; }
        [QueryUI] private Label Text { get; set; }

        public ChallengeView(VisualElement visualElement) : base(visualElement)
        {
            visualElement.style.flexGrow = 1; // TODO: maybe add same style
        }

        public override UniTask Initialize(ChallengeModel model, StoryViewModel viewModel)
        {
            if (model.Day.HasValue)
            {
                Day.text = $"Day {model.Day}";
            }
            else
            {
                Day.style.display = DisplayStyle.None;
            }

            Text.text = $"{model.Text}";
            return UniTask.CompletedTask;
        }
    }
}