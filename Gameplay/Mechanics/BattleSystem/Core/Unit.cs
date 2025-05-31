using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gameplay.Battle
{
    public class Unit
    {
        public string Name { get; }
        public float Health { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public float Mana { get; set; }
        public List<BaseAbility> Abilities { get; } = new List<BaseAbility>();
        public List<BaseEffect> Effects { get; } = new List<BaseEffect>();
        public Unit Target { get; set; }
        public byte TeamID { get; set; }
        public bool IsAlive => Health > 0;

        public Unit(string name)
        {
            Name = name;
        }

        public async UniTask PerformTurn()
        {
            if (!IsAlive) return;

            var availableAbilities = Abilities.Where(a => a.CanUse()).ToList();
            if (availableAbilities.Count > 0)
            {
                var ability = availableAbilities[Random.Range(0, availableAbilities.Count)];
                await ability.Use(this, Target);
            }
        }

        public void UpdateCooldowns(float scaledDeltaTime)
        {
            foreach (var ability in Abilities)
                ability.UpdateCooldown(scaledDeltaTime);

            for (var i = Effects.Count - 1; i >= 0; i--)
            {
                var effect = Effects[i];
                effect.Update(this, scaledDeltaTime);
                effect.UpdateDuration(scaledDeltaTime);

                if (!effect.IsExpired) continue;
                effect.Remove(this);
                Effects.RemoveAt(i);
            }
        }
    }
}