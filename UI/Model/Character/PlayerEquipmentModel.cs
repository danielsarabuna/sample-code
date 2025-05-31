using System;
using System.Collections.Generic;

namespace Model
{
    [Serializable]
    public class PlayerEquipmentModel : CharacterModel
    {
        private static readonly List<EquipmentSlot> InitialEquipmentSlots = new List<EquipmentSlot>
        {
            EquipmentSlot.Helmet,
            EquipmentSlot.Chest,
            EquipmentSlot.Gloves,
            EquipmentSlot.Legs,
            EquipmentSlot.RightHand,
            EquipmentSlot.LeftHand,
            EquipmentSlot.Ring,
            EquipmentSlot.Amulet
        };

        public PlayerEquipmentModel(string name, int level) : base(name, level, InitialEquipmentSlots)
        {
        }

        public PlayerEquipmentModel(string name, int level, Dictionary<EquipmentSlot, EquipmentModel> equipmentSlots) :
            base(name, level, equipmentSlots)
        {
        }
    }
}