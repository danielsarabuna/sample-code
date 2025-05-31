using UnityEngine;

namespace Gameplay.Battle.Effects
{
    public class DamageOverTimeEffect : BaseEffect
    {
        private readonly float _damagePerSecond;
        private float _damageAccumulator;

        public DamageOverTimeEffect(string name, float duration, float damagePerSecond)
        {
            Name = name;
            Duration = duration;
            _damagePerSecond = damagePerSecond;
            _damageAccumulator = 0f;
        }

        public override void Apply(Unit target)
        {
            Debug.Log($"{target.Name} is affected by {Name}!");
        }

        public override void Update(Unit target, float scaledDeltaTime)
        {
            _damageAccumulator += _damagePerSecond * scaledDeltaTime;

            if (_damageAccumulator >= 1f)
            {
                int damage = Mathf.FloorToInt(_damageAccumulator);
                target.Health -= damage;
                _damageAccumulator -= damage;
                Debug.Log($"{target.Name} takes {damage} damage from {Name}!");
            }
        }

        public override void Remove(Unit target)
        {
            Debug.Log($"{Name} effect ended on {target.Name}");
        }
    }
}