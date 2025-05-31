using Cysharp.Threading.Tasks;

namespace Gameplay.Battle
{
    public enum DamageType
    {
        Earth,
        Fire,
        Air,
        Water,
        Crushing, // Дробящие
        Stabbing, // Колищий
        Cutting, // Режуший
        Gunshot, // Огнестрельный
    }

    public abstract class BaseAbility
    {
        private readonly float _cooldown;
        protected readonly string Name;
        private float CurrentCooldown { get; set; }

        protected BaseAbility(string name, float cooldown)
        {
            Name = name;
            _cooldown = cooldown;
            CurrentCooldown = 0;
        }

        public bool CanUse() => CurrentCooldown <= 0;

        public virtual async UniTask Use(Unit user, Unit target)
        {
            if (!CanUse()) return;
            await Execute(user, target);
            CurrentCooldown = _cooldown;
        }

        public void UpdateCooldown(float scaledDeltaTime)
        {
            if (CurrentCooldown > 0)
                CurrentCooldown -= scaledDeltaTime;
        }

        public abstract UniTask Execute(Unit user, Unit target);
    }
}