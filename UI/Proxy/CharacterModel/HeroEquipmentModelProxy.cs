using Model;
using ObservableCollections;

namespace Proxy
{
    public class HeroEquipmentModelProxy : PlayerEquipmentModel
    {
        private readonly CharacterModel _characterModel;
        private readonly ObservableDictionary<EquipmentSlot, EquipmentModel> _equipment;
        public IReadOnlyObservableDictionary<EquipmentSlot, EquipmentModel> ObservableEquipmentSlots => _equipment;

        public HeroEquipmentModelProxy(CharacterModel characterModel) : base(characterModel.Name, characterModel.Level,
            characterModel.EquipmentSlots)
        {
            _characterModel = characterModel;
            _equipment = new ObservableDictionary<EquipmentSlot, EquipmentModel>(_characterModel.EquipmentSlots);
        }

        public override bool Equip(EquipmentModel equipmentModel, EquipmentSlot slot)
        {
            if (!base.Equip(equipmentModel, slot)) return false;
            if (!_characterModel.Equip(equipmentModel, slot)) return false;
            _equipment[slot] = equipmentModel;
            return true;
        }

        public override void TakeOff(EquipmentSlot slot)
        {
            if (!_characterModel.EquipmentSlots.ContainsKey(slot)) return;
            base.TakeOff(slot);
            _characterModel.TakeOff(slot);
            _equipment[slot] = null;
        }
    }
}