using System;

namespace Model
{
    public enum WeaponType
    {
        Sword, // Меч
        Bow, // Лук
        Staff, // Посох
        Spear, // Копье
        Shield, // Щит
    }

    [Serializable]
    public class WeaponModel : EquipmentModel
    {
        public WeaponType Type { get; set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }

        public WeaponModel() : base(string.Empty, 1, EquipmentRarity.Common)
        {
        }

        public WeaponModel(string name, uint level, EquipmentRarity rarity, WeaponType type, float damage,
            float attackSpeed) : base(name, level, rarity)
        {
            Type = type;
            Damage = damage;
            AttackSpeed = attackSpeed;
        }
    }
}