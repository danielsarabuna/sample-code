using System;
using System.Collections.Generic;
using Ink.Runtime;

namespace Gameplay
{
    public interface IChallengeRunnerConfig
    {
        void SetEventHandler(Action<string> eventHandler);
        void SetChoiceHandler(Action<int> choiceHandler);
        void SetNeedEventHandler(Action needEventHandler);
        void SetChoiceDecisionHandler(Action<List<Choice>> choiceDecisionMaker);
        void SetTagHandler(Action<IReadOnlyDictionary<string, string>> tags);
    }
}