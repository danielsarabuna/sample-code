using Cysharp.Threading.Tasks;

namespace Gameplay
{
    public interface IChallengeRunner : IChallengeRunnerConfig
    {
        bool IsCanContinue { get; }

        UniTask Init();
        void GoToEvent(string eventName);
        void MakeChoice(int choiceIndex);
    }
}