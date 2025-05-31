using System.Threading.Tasks;
using UnityEngine;

namespace Gameplay
{
    public class GameExample
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/GameExample")]
#endif
        public static async void Main()
        {
            // --- Инициализация ---
            Debug.Log("Game Started!");

            // Создаем персонажей
            var player = new TestActor("Player");
            var skeleton = new TestActor("Skeleton");

            // Подключаем компоненты
            player.AddComponent(new EffectComponent(player));
            player.AddComponent(new StatsComponent(player));
            player.AddComponent(new AIComponent(player));

            skeleton.AddComponent(new EffectComponent(skeleton));
            skeleton.AddComponent(new StatsComponent(skeleton));
            skeleton.AddComponent(new AIComponent(skeleton));

            player.GetComponent<StatsComponent>().ModifyStat(StatType.Health, new StatModifier(ModifierType.Flat, 100));
            player.GetComponent<StatsComponent>().ModifyStat(StatType.Speed, new StatModifier(ModifierType.Flat, 1.5F));
            skeleton.GetComponent<StatsComponent>()
                .ModifyStat(StatType.Health, new StatModifier(ModifierType.Flat, 50));

            // --- Подписка на события ---
            GlobalEventManager.Subscribe("EffectApplied",
                (eventData) =>
                {
                    Debug.Log(
                        $"[Event: Effect Applied] {eventData.Source.Name} получил эффект: {((IEffect)eventData.Payload).Name}");
                });

            GlobalEventManager.Subscribe("StatModified",
                (eventData) =>
                {
                    Debug.Log(
                        $"[Event: Stat Modified] {eventData.Source.Name} характеристика изменена: {eventData.Payload}");
                });

            GlobalEventManager.Subscribe("CharacterHurt",
                (eventData) => { Debug.Log($"[Event: Hurt] {eventData.Source.Name} ранен!"); });

            // --- Эффекты ---
            var slowEffect = new Effect("Slow", 5f,
                (target) => Debug.Log($"Effect 'Slow' applied to {target.Name}: Speed reduced!"),
                (target) => Debug.Log($"Effect 'Slow' removed from {target.Name}: Speed restored!"),
                (target, deltaTime) =>
                {
                    var statsComponent = target.GetComponent<StatsComponent>();
                    statsComponent.ModifyStat(StatType.Speed, new StatModifier(ModifierType.Flat, 0.1F));
                    var stat = statsComponent.GetFinalStat(StatType.Speed);
                    Debug.Log($"Effect 'Slow' ticking on {target.Name}: -0.1({stat}) speed per second.");
                }
            );

            var poisonEffect = new Effect("Poison", 4f,
                (target) => Debug.Log($"Effect 'Poison' applied to {target.Name}: Losing health over time."),
                (target) => Debug.Log($"Effect 'Poison' removed from {target.Name}: Poison cleared."),
                (target, deltaTime) =>
                {
                    var statsComponent = target.GetComponent<StatsComponent>();
                    statsComponent.ModifyStat(StatType.Health, new StatModifier(ModifierType.Flat, 5));
                    var stat = statsComponent.GetFinalStat(StatType.Health);
                    Debug.Log($"Effect 'Poison' ticking on {target.Name}: {stat} HP remaining.");
                }
            );

            // Применяем эффекты
            player.GetComponent<EffectComponent>().AddEffect(slowEffect, player);
            skeleton.GetComponent<EffectComponent>().AddEffect(poisonEffect, skeleton);

            // --- AI Переходы ---
            var playerAI = player.GetComponent<AIComponent>();
            playerAI.Initialize();

            var skeletonAI = skeleton.GetComponent<AIComponent>();
            skeletonAI.Initialize();

            // --- Модификаторы статов ---
            var strengthBoost = new StatModifier(ModifierType.Flat, 10f);
            var agilityDebuff = new StatModifier(ModifierType.Percent, -0.3f);

            player.GetComponent<StatsComponent>().ModifyStat(StatType.Strength, strengthBoost);
            player.GetComponent<StatsComponent>().ModifyStat(StatType.Agility, agilityDebuff);

            // --- Игровой цикл (обновление) ---
            for (int i = 0; i < 10; i++)
            {
                Debug.Log($"--- Frame {i + 1} ---");

                // Отображение текущего состояния
                Debug.Log($"Player Health: {player.GetComponent<StatsComponent>().GetFinalStat(StatType.Health)}");
                Debug.Log($"Skeleton Health: {skeleton.GetComponent<StatsComponent>().GetFinalStat(StatType.Health)}");

                // Искусственно разыгрываем события
                if (i == 5)
                {
                    GlobalEventManager.Trigger(new GameEvent("CharacterHurt", skeleton));
                }

                player.Update();
                skeleton.Update();
                // Симуляция времени
                await Task.Delay(500);
            }

            Debug.Log("Game Ended!");
        }
    }
}