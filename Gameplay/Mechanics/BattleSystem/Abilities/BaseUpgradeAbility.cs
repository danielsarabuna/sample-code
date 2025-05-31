using Cysharp.Threading.Tasks;

namespace Gameplay.Battle.Abilities
{
    public abstract class BaseUpgradeAbility : BaseAbility
    {
        private readonly BaseAbility _baseAbility;

        protected BaseUpgradeAbility(string name, float cooldown, BaseAbility baseAbility) : base(name, cooldown)
        {
            _baseAbility = baseAbility;
        }

        public override async UniTask Execute(Unit user, Unit target)
        {
            await _baseAbility.Execute(user, target);
        }
    }
}