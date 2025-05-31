using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Battle.Abilities
{
    public class HealAbility : BaseAbility
    {
        private readonly float _healAmount;

        public HealAbility(string name, float cooldown, float healAmount) : base(name, cooldown)
        {
            _healAmount = healAmount;
        }

        public override UniTask Execute(Unit user, Unit target)
        {
            user.Health += _healAmount;
            Debug.Log($"{user.Name} uses {Name} and heals for {_healAmount}");
            return UniTask.CompletedTask;
        }
    }
}