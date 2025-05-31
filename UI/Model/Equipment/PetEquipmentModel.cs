using System;

namespace Model
{
    public enum PetEquipmentType
    {
        Collar,
        Amulet,
        Helmet
    }

    [Serializable]
    public class PetEquipmentModel : EquipmentModel
    {
        public PetEquipmentType Type { get; set; }

        public PetEquipmentModel() : base(string.Empty, 1, EquipmentRarity.Common)
        {
        }

        public PetEquipmentModel(string name, uint level, EquipmentRarity rarity, PetEquipmentType type) : base(name,
            level, rarity)
        {
            Type = type;
        }
    }
}