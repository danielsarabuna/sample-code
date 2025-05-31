namespace Gameplay.Battle
{
    public abstract class BaseEffect
    {
        public string Name { get; protected set; }
        public float Duration { get; protected set; }
        public bool IsExpired => Duration <= 0;

        public abstract void Apply(Unit target);
        public abstract void Remove(Unit target);
        public abstract void Update(Unit target, float scaledDeltaTime);

        public void UpdateDuration(float scaledDeltaTime)
        {
            Duration -= scaledDeltaTime;
        }
    }
}