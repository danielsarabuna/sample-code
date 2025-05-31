using Cysharp.Threading.Tasks;
using Model;
using Service;

namespace Executor.Command
{
    public class UpgradeStatCommand : ICommand
    {
        private readonly IDebugService _debugService;
        private readonly IUserGameDetails _userGameDetails;
        private readonly StatModel _statModel;
        private readonly IProgressionCalculator _calculatorValue;
        private readonly IProgressionCalculator _calculatorCost;
        private readonly uint _value;

        public UpgradeStatCommand(IDebugService debugService, IUserGameDetails userGameDetails, StatModel statModel,
            IProgressionCalculator calculatorValue, IProgressionCalculator calculatorCost)
        {
            _debugService = debugService;
            _userGameDetails = userGameDetails;
            _statModel = statModel;
            _calculatorValue = calculatorValue;
            _calculatorCost = calculatorCost;
        }

        public UniTask<Status> Execute()
        {
            if (_statModel.IsMaxLevel.CurrentValue)
                return UniTask.FromResult(Status.Failure); // TODO: will add currency
            _statModel.Level.Value++;
            var currentLevel = _statModel.Level.CurrentValue;
            _statModel.CurrentValue.Value = _calculatorValue.GetValue(currentLevel);
            _statModel.UpgradeValue.Value = _calculatorValue.GetValue(currentLevel + 1) - _statModel.CurrentValue.Value;
            _statModel.UpgradeCost.Value = _calculatorCost.GetValue(currentLevel);
            var key = _statModel.Name.CurrentValue.ToLower();
            _userGameDetails.GameState.Set(key, _statModel.Level.CurrentValue);
            _debugService.Log("Stat upgraded successfully.");
            return UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            return UniTask.FromResult(Status.Success);
        }
    }
}