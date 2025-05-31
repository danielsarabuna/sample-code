using System;

namespace Model.Actor
{
    using System.Collections.Generic;

    // Базовые модели персонажей
    public abstract class ActorModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        protected ActorModel(string name, int level)
        {
            Name = name;
            Level = level;
        }
    }

    // Модели конкретных персонажей
    public class Stats
    {
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Vitality { get; set; }
    }

    public class Progression
    {
        public int Experience { get; set; }
        public int SkillPoints { get; set; }
        public List<string> UnlockedAbilities { get; set; }
    }

    [Serializable]
    public class HeroModel : ActorModel
    {
        public Stats BaseStats { get; set; }
        public Progression Progress { get; set; }
        public List<string> EquippedItems { get; set; }
        public List<string> ActivePets { get; set; }

        public HeroModel(string name, int level, Stats baseStats, Progression progress, List<string> equippedItems,
            List<string> activePets) : base(name, level)
        {
            BaseStats = baseStats;
            Progress = progress;
            EquippedItems = equippedItems;
            ActivePets = activePets;
        }
    }

    [Serializable]
    public class BehaviorParams
    {
        public float AggroRange { get; set; }
        public float AttackRange { get; set; }
        public float AttackCooldown { get; set; }
        public bool IsAggressive { get; set; }
        public List<string> PreferredTargets { get; set; } = new List<string>();
        public List<string> SpecialAbilities { get; set; } = new List<string>();
    }

    [Serializable]
    public class LootTable
    {
        public List<LootEntry> PossibleLoot { get; set; } = new List<LootEntry>();
        public float ExperienceValue { get; set; }
    }

    [Serializable]
    public class EnemyModel : ActorModel
    {
        public BehaviorParams Behavior { get; set; }
        public LootTable Loot { get; set; }
        public string Faction { get; set; }

        public EnemyModel() : base(string.Empty, 1)
        {
            Behavior = new BehaviorParams();
            Loot = new LootTable();
        }

        public EnemyModel(string name, int level, BehaviorParams behavior, LootTable loot, string faction) : base(name,
            level)
        {
            Behavior = behavior;
            Loot = loot;
            Faction = faction;
        }
    }

    [Serializable]
    public class PetStats
    {
        public float Loyalty { get; set; }
        public float BondLevel { get; set; }
        public List<string> SpecialAbilities { get; set; } = new List<string>();
    }

    [Serializable]
    public class PetModel : ActorModel
    {
        public PetStats Stats { get; set; }
        public string OwnerID { get; set; }
        public PetType PetType { get; set; }
        public GrowthStage Stage { get; set; }

        public PetModel() : base(string.Empty, 1)
        {
            Stats = new PetStats();
        }

        public PetModel(string name, int level, PetStats stats, string ownerID, PetType petType, GrowthStage stage) :
            base(name, level)
        {
            Stats = stats;
            OwnerID = ownerID;
            PetType = petType;
            Stage = stage;
        }
    }

    [Serializable]
    public class TraderInfo
    {
        public List<string> AvailableItems { get; set; } = new List<string>();
        public float PriceMultiplier { get; set; }
        public string SpecialCurrency { get; set; }
    }

    [Serializable]
    public class QuestInfo
    {
        public List<string> AvailableQuests { get; set; } = new List<string>();
        public List<string> CompletedQuests { get; set; } = new List<string>();
    }

    [Serializable]
    public class NpcModel : ActorModel
    {
        public NpcType Role { get; set; }
        public TraderInfo TradeData { get; set; }
        public QuestInfo QuestData { get; set; }
        public List<string> DialogueTree { get; set; } = new List<string>();

        public NpcModel() : base(string.Empty, 1)
        {
            TradeData = new TraderInfo();
            QuestData = new QuestInfo();
        }

        public NpcModel(string name, int level, NpcType role, TraderInfo tradeData, QuestInfo questData,
            List<string> dialogueTree) : base(name, level)
        {
            Role = role;
            TradeData = tradeData;
            QuestData = questData;
            DialogueTree = dialogueTree;
        }
    }

    // Пример фабрики для создания персонажей

    // Пример использования
    // public class GameWorld
    // {
    //     private CharacterModelFactory _factory = new CharacterModelFactory();
    //     private Dictionary<string, ActorModel> _characters = new Dictionary<string, ActorModel>();
    //
    //     public void CreateGameWorld()
    //     {
    //         // Создаем героя
    //         var hero = _factory.CreateHeroModel("Hero", HeroClass.Warrior);
    //         _characters[hero.Id] = hero;
    //
    //         // Создаем врагов
    //         var goblin = _factory.CreateEnemyModel("Goblin", 1);
    //         _characters[goblin.Id] = goblin;
    //
    //         // Создаем питомца
    //         var pet = _factory.CreatePetModel(PetType.Dragon, hero.Id);
    //         _characters[pet.Id] = pet;
    //     }
    // }

    // Перечисления

    public enum HeroClass
    {
        Warrior,
        Mage,
        Ranger,
        Rogue
    }

    public enum PetType
    {
        Dragon,
        Wolf,
        Phoenix,
        Sprite
    }

    public enum GrowthStage
    {
        Young,
        Adult,
        Elder,
        Legendary
    }

    public enum NpcType
    {
        Merchant,
        QuestGiver,
        Trainer,
        Craftsman
    }

    [Serializable]
    // Структура для лута
    public class LootEntry
    {
        public string ItemId { get; set; }
        public float DropChance { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }
}