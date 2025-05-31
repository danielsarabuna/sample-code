using System.Collections.Generic;

namespace Model
{
    public class InventoryModel
    {
        public List<EquipmentModel> Items { get; private set; } = new List<EquipmentModel>();

        public void AddItem(EquipmentModel equipment)
        {
            Items.Add(equipment);
        }

        public void RemoveItem(EquipmentModel equipment)
        {
            Items.Remove(equipment);
        }
    }
}