using System;

namespace Model
{
    public enum AccessoryType
    {
        Ring, // Кольцо
        Amulet, // Амулет
        Headband // Повязка на голову
    }

    [Serializable]
    public class AccessoryModel : EquipmentModel
    {
        public AccessoryType Type { get; set; }
        public string UniqueAbility { get; set; }

        public AccessoryModel() : base(string.Empty, 1, EquipmentRarity.Common)
        {
        }

        public AccessoryModel(string name, uint level, EquipmentRarity rarity, AccessoryType type, string uniqueAbility)
            : base(name, level, rarity)
        {
            Type = type;
            UniqueAbility = uniqueAbility;
        }
    }
}