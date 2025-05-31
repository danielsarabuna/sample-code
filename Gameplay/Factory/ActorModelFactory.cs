using System;
using System.Collections.Generic;
using Model;
using Model.Actor;
using Service;
using VContainer;
using HeroModel = Model.Actor.HeroModel;

namespace Gameplay.Factory
{
    public class ActorModelFactory
    {
        private ProgressionService _progression;

        public ActorModelFactory(IObjectResolver resolver)
        {
            _progression = resolver.Resolve<ProgressionService>();
        }

        public HeroModel CreateHeroModel(IUserGameDetails userGameDetails, string name, HeroClass heroClass)
        {
            var stats = CreateBaseStatsForClass(heroClass);
            var progression = CreateProgressionForHero();
            var model = new HeroModel(name, 1, stats, progression, new List<string>(), new List<string>());
            var stateData = userGameDetails.GameState.StateData;
            var healthLevel = Convert.ToUInt32(stateData["health"]);
            var attackLevel = Convert.ToUInt32(stateData["attack"]);
            var defenseLevel = Convert.ToUInt32(stateData["defense"]);

            model.Data["Health"] = CalculateStatCurrentValue("health", healthLevel);
            model.Data["Attack"] = CalculateStatCurrentValue("attack", attackLevel);
            model.Data["Defense"] = CalculateStatCurrentValue("defense", defenseLevel);
            return model;
        }

        private ulong CalculateStatCurrentValue(string stat, uint level)
        {
            return _progression.Get<IProgressionCalculator>($"{stat}-value").GetValue(level);
        }

        public EnemyModel CreateEnemyModel(string enemyType, int level)
        {
            var enemy = new EnemyModel($"{enemyType}_{level}", level, CreateBehaviorForType(enemyType),
                CreateLootTableForType(enemyType, level), DetermineFaction(enemyType));
            return enemy;
        }

        private string DetermineFaction(string enemyType)
        {
            return enemyType.ToLower() switch
            {
                "goblin" or "orc" or "bandit" => "Bandit", // Человекообразные враги
                "wolf" or "bear" or "spider" => "Beast", // Дикие звери
                "skeleton" or "zombie" or "lich" => "Undead", // Нежить
                "fire elemental" or "ice elemental" or "storm elemental" => "Elemental", // Элементали
                "imp" or "demon lord" or "succubus" => "Demon", // Демоны
                _ => "Enemy"
            };
        }

        public PetModel CreatePetModel(PetType type, string ownerID)
        {
            var petStats = new PetStats
            {
                Loyalty = 0,
                BondLevel = 0,
                SpecialAbilities = GetInitialAbilitiesForPetType(type)
            };
            return new PetModel("", 1, petStats, string.Empty, type, GrowthStage.Young);
        }

        private Stats CreateBaseStatsForClass(HeroClass heroClass)
        {
            // Реализация создания базовых характеристик
            return new Stats();
        }

        private Progression CreateProgressionForHero()
        {
            return new Progression
            {
                Experience = 0,
                SkillPoints = 0,
                UnlockedAbilities = new List<string>()
            };
        }

        private BehaviorParams CreateBehaviorForType(string enemyType)
        {
            // Реализация создания поведения
            return new BehaviorParams();
        }

        private LootTable CreateLootTableForType(string enemyType, int level)
        {
            // Реализация создания таблицы лута
            return new LootTable();
        }

        private List<string> GetInitialAbilitiesForPetType(PetType type)
        {
            // Реализация получения начальных способностей
            return new List<string>();
        }
    }
}