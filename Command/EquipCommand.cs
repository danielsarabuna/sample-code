using System.Linq;
using Cysharp.Threading.Tasks;
using Model;

namespace Executor.Command
{
    public class EquipCommand : ICommand
    {
        private readonly CharacterModel _characterModel;
        private readonly EquipmentModel _equipment;

        public EquipCommand(CharacterModel characterModel, EquipmentModel equipment)
        {
            _characterModel = characterModel;
            _equipment = equipment;
        }

        public UniTask<Status> Execute()
        {
            return _characterModel.EquipmentSlots.Keys
                .Where(equipmentSlot => equipmentSlot.CanEquip(_equipment))
                .Any(equipmentSlot => _characterModel.Equip(_equipment, equipmentSlot))
                ? new UniTask<Status>(Status.Success)
                : new UniTask<Status>(Status.Failure);
        }

        public UniTask<Status> Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}