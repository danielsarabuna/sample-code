using System;
using System.Collections.Generic;

namespace Gameplay
{
    public class EventSystem : IEventSystem
    {
        private readonly Queue<string> _eventHistory;
        private readonly int _maxHistorySize;

        public EventSystem(int maxHistorySize = 3)
        {
            _maxHistorySize = maxHistorySize;
            _eventHistory = new Queue<string>(_maxHistorySize);
        }

        public void ProcessEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            AddToHistory(eventName);
        }

        public bool IsEventAllowed(string eventName, int roll)
        {
            return !IsInHistory(eventName);
        }

        public void AddToHistory(string eventName)
        {
            if (_eventHistory.Count >= _maxHistorySize)
                _eventHistory.Dequeue();

            _eventHistory.Enqueue(eventName);
        }

        public bool IsInHistory(string eventName)
        {
            return _eventHistory.Contains(eventName);
        }
    }
}