using System;
using Cysharp.Threading.Tasks;
using Model;
using Service;

namespace Executor.Command
{
    public class AddExperienceCommand : ICommand
    {
        private readonly IDebugService _debug;
        private readonly IUserGameDetails _gameDetails;
        private readonly IProgressionCalculator _calculator;
        private readonly ulong _value;

        public AddExperienceCommand(IDebugService debug, IUserGameDetails gameDetails,
            ProgressionService service, ulong value)
        {
            _debug = debug;
            _gameDetails = gameDetails;
            _calculator = service.Get<IProgressionCalculator>("experience");
            _value = value;
        }

        public UniTask<Status> Execute()
        {
            var currentValue = Convert.ToUInt64(_gameDetails.GameState.StateData["experience"]);
            _debug.Log($"Adding {_value} experience. Current experience: {currentValue}");
            _gameDetails.GameState.StateData["experience"] = currentValue + _value;
            TryLevelUp();
            return UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            return UniTask.FromResult(Status.Success);
        }

        private void TryLevelUp()
        {
            var currentExperience = Convert.ToUInt64(_gameDetails.GameState.Get("experience"));
            var currentLevel = Convert.ToUInt64(_gameDetails.GameState.Get("level"));

            var expToNextLevel = _calculator.GetValue(currentLevel);
            while (currentExperience >= expToNextLevel)
            {
                _debug.Log($"Leveling up from level {currentLevel}. CurrentExp: {currentExperience}, ExpToNextLevel: {expToNextLevel}");
                currentLevel++;
                currentExperience -= expToNextLevel;
                _debug.Log($"Leveled up! New level: {currentLevel}, Remaining experience: {currentExperience}");
                ApplyLevelUpBonus(currentLevel);
                expToNextLevel = _calculator.GetValue(currentLevel);
            }

            _gameDetails.GameState.Set("experience", currentExperience);
            _gameDetails.GameState.Set("level", currentLevel);
        }

        private void ApplyLevelUpBonus(ulong level)
        {
            _debug.Log($"Applying level-up bonus for level {level}.");
            if (level % 10 != 0) return;
            var prestige = Convert.ToUInt64(_gameDetails.GameState.Get("prestige"));
            prestige++;
            _gameDetails.GameState.Set("prestige", prestige);
            _debug.Log($"Prestige increased! New prestige: {prestige}");
            UnlockSpecialAbility(level);
        }

        private void UnlockSpecialAbility(ulong level)
        {
            _debug.Log($"Unlocking special ability for level {level}!");
        }
    }
}