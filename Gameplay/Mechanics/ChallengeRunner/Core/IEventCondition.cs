namespace Gameplay
{
    public interface IEventCondition
    {
        string Name { get; }
        bool IsAllowed(int rollValue);
    }
}