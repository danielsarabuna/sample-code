using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Battle.Abilities
{
    public class DamageAbility : BaseAbility
    {
        private readonly float _damage;

        public DamageAbility(string name, float cooldown, float damage) : base(name, cooldown)
        {
            _damage = damage;
        }

        public override UniTask Execute(Unit user, Unit target)
        {
            target.Health -= _damage;
            Debug.Log($"{user.Name} uses {Name} and deals {_damage} damage to {target.Name}");
            return UniTask.CompletedTask;
        }
    }
}