using System.Linq;
using Cysharp.Threading.Tasks;
using Model;

namespace Executor.Command
{
    public class TakeOffCommand : ICommand
    {
        private readonly CharacterModel _characterModel;
        private readonly EquipmentModel _equipment;

        public TakeOffCommand(CharacterModel characterModel, EquipmentModel equipment)
        {
            _characterModel = characterModel;
            _equipment = equipment;
        }

        public UniTask<Status> Execute()
        {
            if (!_characterModel.IsEquipped(_equipment)) return new UniTask<Status>(Status.Failure);
            var pair = _characterModel.EquipmentSlots
                .Where(x => x.Value != null)
                .FirstOrDefault(x => x.Value.Name == _equipment.Name);
            _characterModel.TakeOff(pair.Key);
            return new UniTask<Status>(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}