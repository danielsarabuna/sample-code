using Gameplay.Battle.Abilities;

namespace Gameplay.Battle
{
    public class AbilityFactory
    {
        public BaseAbility CreateDamage(string name, float cooldown, float damage)
            => new DamageAbility(name, cooldown, damage);

        public BaseAbility CreateHeal(string name, float cooldown, float healAmount)
            => new HealAbility(name, cooldown, healAmount);

        public BaseAbility CreateDot(string name, float cooldown, float duration, float damagePerSecond)
            => new DotAbility(name, cooldown, duration, damagePerSecond);

        public BaseAbility CreateDamageAddition(string name, float cooldown, BaseAbility baseAbility, float damage)
            => new DamageAdditionAbility(name, cooldown, baseAbility, damage);
    }
}