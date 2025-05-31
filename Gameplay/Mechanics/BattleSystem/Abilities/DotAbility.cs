using Cysharp.Threading.Tasks;
using Gameplay.Battle.Effects;
using UnityEngine;

namespace Gameplay.Battle.Abilities
{
    public class DotAbility : BaseAbility
    {
        private readonly float _duration;
        private readonly float _damagePerSecond;

        public DotAbility(string name, float cooldown, float duration, float damagePerSecond) : base(name, cooldown)
        {
            _duration = duration;
            _damagePerSecond = damagePerSecond;
        }

        public override UniTask Execute(Unit user, Unit target)
        {
            var effect = new DamageOverTimeEffect(Name, _duration, _damagePerSecond);
            effect.Apply(target);
            target.Effects.Add(effect);
            Debug.Log($"{user.Name} applies {Name} to {target.Name}");
            return UniTask.CompletedTask;
        }
    }
}