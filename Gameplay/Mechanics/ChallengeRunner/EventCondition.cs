using System;
using Model;

namespace Gameplay
{
    public class EventCondition : IEventCondition
    {
        private readonly StoryModel _model;
        private readonly Func<StoryModel, bool> _func;
        public string Name { get; }
        private readonly float? _rollThreshold = 100F;

        public EventCondition(string name, StoryModel model, Func<StoryModel, bool> func)
        {
            _model = model;
            Name = name;
            _func = func;
        }

        public EventCondition(string name, StoryModel model, int rollThreshold, Func<StoryModel, bool> func)
        {
            Name = name;
            _rollThreshold = rollThreshold;
            _model = model;
            _func = func;
        }

        public bool IsAllowed(int rollValue)
        {
            return rollValue <= _rollThreshold && _func.Invoke(_model);
        }
    }
}