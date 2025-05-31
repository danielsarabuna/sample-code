using System;
using System.Collections.Generic;

namespace Model
{
    public enum EquipmentRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum EquipmentEffectType
    {
        None = 0,
        MovementSpeed, // увеличение скорости передвижения
        DamageReduction, // снижение получаемого урона
        Reflection, // отражение урона
        FireResistance, // сопротивление огню
        Regeneration, // восстановление здоровья
        MagicImmunity, // шанс игнорировать магический урон
        DivineShield, // шанс блокировать урон
        CriticalStrike, // шанс критического удара
        FireDamage, // дополнительный урон огнём
        LightningChain, // цепная молния
        LifeSteal, // вампиризм
        HealingBoost, // усиление получаемого лечения
        SpellPower, // усиление магического урона
        ManaRegeneration, // восстановление маны
        CriticalDamage, // усиление критического урона
        Resurrection, // воскрешение
        ElementalMastery, // усиление стихийного урона
        Freeze, // заморозка противника
        SoulDrain, // похищение здоровья
        PhaseShift, // уклонение от атак
        CooldownReduction, // снижение перезарядки
        RealityWarp, // дублирование заклинаний
        ChaosMagic, // случайные магические эффекты
        NatureBlessing, // природное благословение
        OmnipotenceBoost, // усиление всех характеристик
        Burn, // эффект горения
    }

    [Serializable]
    public class EquipmentEffectModel
    {
        public EquipmentEffectType EffectType { get; set; }
        public float Value { get; set; }
        public float Duration { get; set; }
        public string Description { get; set; }

        public EquipmentEffectModel(EquipmentEffectType effectType, float value, float duration)
        {
            EffectType = effectType;
            Value = value;
            Duration = duration;
        }
    }

    public abstract class EquipmentModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string AssetPath { get; set; }
        public string Description { get; set; }
        public uint Level { get; set; }
        public EquipmentRarity Rarity { get; set; }
        public List<EquipmentEffectModel> Effects { get; set; }

        protected EquipmentModel(string name, uint level, EquipmentRarity rarity)
        {
            Name = name;
            Level = level;
            Rarity = rarity;
            Effects = new List<EquipmentEffectModel>();
        }
    }
}