using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Battle.Abilities
{
    public class DamageAdditionAbility : BaseUpgradeAbility
    {
        private readonly float _damage;

        public DamageAdditionAbility(string name, float cooldown, BaseAbility baseAbility, float damage)
            : base(name, cooldown, baseAbility)
        {
            _damage = damage;
        }

        public override async UniTask Execute(Unit user, Unit target)
        {
            await base.Execute(user, target);

            target.Health -= _damage;
            Debug.Log($"{user.Name} uses {Name} and deals {_damage} damage to {target.Name}");
        }
    }
}