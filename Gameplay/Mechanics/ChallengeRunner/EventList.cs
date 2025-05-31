using System.Collections;
using System.Collections.Generic;
using Model;

namespace Gameplay
{
    public class EventList : IEnumerable<IEventCondition>
    {
        public EventCondition End { get; }
        public EventCondition SuccessfulCompletion { get; }
        public EventCondition Shop { get; }
        public EventCondition BossBattle { get; }
        public EventCondition EnemyBattle { get; }
        public EventCondition Treasure { get; }
        public EventCondition AllyMeeting { get; }
        public EventCondition HealthPromotion { get; }
        public EventCondition HealthLoss { get; }
        public EventCondition AttackPromotion { get; }
        public EventCondition AttackLoss { get; }
        public EventCondition Random { get; }

        public EventList(StoryModel model)
        {
            End = new EventCondition("event_end", model, x => x.PlayerHealth <= 0);
            SuccessfulCompletion = new EventCondition("event_successful_completion", model, x => x.CurrentDay >= 49);
            Shop = new EventCondition("event_shop", model, x => false);
            BossBattle = new EventCondition("event_boss_battle", model, x => x.CurrentDay % 10 == 0);
            EnemyBattle = new EventCondition("event_enemy_battle", model, x => true); // TODO: return x.CurrentDay % 5 == 0
            Treasure = new EventCondition("event_treasure", model, 70, x => true);
            AllyMeeting = new EventCondition("event_ally_meeting", model, 85, x => true);
            HealthPromotion = new EventCondition("event_health_promotion", model, x => x.PlayerMaxHealth / 100F * 50 <= x.PlayerHealth);
            HealthLoss = new EventCondition("event_health_loss", model, x => x.PlayerMaxHealth / 100F * 80 >= x.PlayerHealth);
            AttackPromotion = new EventCondition("event_attack_promotion", model, x => x.PlayerMaxAttack / 100F * 50 <= x.PlayerAttack);
            AttackLoss = new EventCondition("event_attack_loss", model, x => x.PlayerMaxAttack / 100F * 50 <= x.PlayerAttack);
            Random = new EventCondition("event_random", model, x => true);
        }

        public IEnumerator<IEventCondition> GetEnumerator()
        {
            yield return End;
            yield return SuccessfulCompletion;
            yield return Shop;
            yield return BossBattle;
            yield return EnemyBattle;
            yield return Treasure;
            yield return AllyMeeting;
            yield return HealthPromotion;
            yield return HealthLoss;
            yield return AttackPromotion;
            yield return AttackLoss;
            yield return Random;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}