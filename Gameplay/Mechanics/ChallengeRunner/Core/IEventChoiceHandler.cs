using System.Collections.Generic;
using Ink.Runtime;

namespace Gameplay
{
    public interface IEventChoiceHandler
    {
        int MakeChoice(List<Choice> currentChoices);
    }
}