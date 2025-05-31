using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public enum EquipmentSlot
    {
        Helmet, // Шлемы
        Chest, // Нагрудник / Броня
        Gloves, // Перчатки
        Legs, // Ноги
        RightHand, // Оружие (мечи, луки, посохи)
        LeftHand, // Оружие (мечи, луки, посохи, Щиты)
        Ring, // Кольцо (аксессуар)
        Amulet, // Амулет (аксессуар)
        Collar, // Ошейник (для питомцев)
        PetHelmet // Шлем для питомца
    }

    public abstract class CharacterModel
    {
        public readonly string Name;
        public readonly int Level;
        public Dictionary<EquipmentSlot, EquipmentModel> EquipmentSlots { get; }

        protected CharacterModel(string name, int level, List<EquipmentSlot> availableSlots)
        {
            Name = name;
            Level = level;

            EquipmentSlots = new Dictionary<EquipmentSlot, EquipmentModel>();
            foreach (var slot in availableSlots)
                EquipmentSlots.Add(slot, null);
        }

        protected CharacterModel(string name, int level, Dictionary<EquipmentSlot, EquipmentModel> equipmentSlots)
        {
            Name = name;
            Level = level;

            EquipmentSlots = equipmentSlots;
        }

        public virtual bool Equip(EquipmentModel equipmentModel, EquipmentSlot slot)
        {
            if (!EquipmentSlots.ContainsKey(slot)) return false;
            EquipmentSlots[slot] = equipmentModel;
            return true;
        }

        public virtual void TakeOff(EquipmentSlot slot)
        {
            if (EquipmentSlots.ContainsKey(slot))
                EquipmentSlots[slot] = null;
        }

        public bool IsEquipped(EquipmentModel equipmentModel)
        {
            return EquipmentSlots.Values
                .Where(x => x != null)
                .Any(x => x.Name == equipmentModel.Name);
        }
    }

    public static class EquipmentSlotExtensions
    {
        public static bool CanEquip(this EquipmentSlot slot, EquipmentModel equipment)
        {
            return slot switch
            {
                EquipmentSlot.Helmet => equipment is ArmorModel { Type: ArmorType.Helmet },
                EquipmentSlot.Chest => equipment is ArmorModel { Type: ArmorType.Chest },
                EquipmentSlot.Gloves => equipment is ArmorModel { Type: ArmorType.Gloves },
                EquipmentSlot.Legs => equipment is ArmorModel { Type: ArmorType.Legs },
                EquipmentSlot.LeftHand => equipment is WeaponModel,
                EquipmentSlot.RightHand => equipment is WeaponModel,
                EquipmentSlot.Ring or EquipmentSlot.Amulet => equipment is AccessoryModel,
                EquipmentSlot.Collar => equipment is PetEquipmentModel { Type: PetEquipmentType.Collar },
                EquipmentSlot.PetHelmet => equipment is PetEquipmentModel { Type: PetEquipmentType.Helmet },
                _ => false
            };
        }
    }
}