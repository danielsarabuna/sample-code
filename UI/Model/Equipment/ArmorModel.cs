using System;

namespace Model
{
    public enum ArmorType
    {
        Helmet, // Шлемы
        Chest, // Нагрудник 
        Gloves, // Перчатки
        Legs, // Ноги
    }

    [Serializable]
    public class ArmorModel : EquipmentModel
    {
        public ArmorType Type { get; set; }
        public float Defense { get; set; }
        public float HealthBonus { get; set; }

        public ArmorModel() : base(string.Empty, 1, EquipmentRarity.Common)
        {
        }

        public ArmorModel(string name, uint level, EquipmentRarity rarity, ArmorType type, float defense,
            float healthBonus) : base(name, level, rarity)
        {
            Type = type;
            Defense = defense;
            HealthBonus = healthBonus;
        }
    }
}