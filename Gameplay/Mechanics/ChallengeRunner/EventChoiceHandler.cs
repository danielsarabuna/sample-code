using System.Collections.Generic;
using Ink.Runtime;

namespace Gameplay
{
    public class EventChoiceHandler : IEventChoiceHandler
    {
        public int MakeChoice(List<Choice> currentChoices)
        {
            return UnityEngine.Random.Range(0, currentChoices.Count);
        }
    }
}