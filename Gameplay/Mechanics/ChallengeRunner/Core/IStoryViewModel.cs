using Model;
using R3;

namespace Gameplay
{
    public interface IStoryViewModel
    {
        ReadOnlyReactiveProperty<bool> IsMoveActive { get; }
        void UpdateView(ChallengeModel model);
    }
}