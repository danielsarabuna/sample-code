namespace Gameplay
{
    public interface IEventSystem
    {
        void ProcessEvent(string eventName);
        bool IsEventAllowed(string eventName, int roll);
        void AddToHistory(string eventName);
        bool IsInHistory(string eventName);
    }
}