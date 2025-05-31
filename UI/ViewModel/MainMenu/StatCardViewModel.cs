using Executor.Command;
using Model;
using R3;

namespace ViewModel
{
    public class StatCardViewModel
    {
        private readonly ICommandExecutor _performer;
        private readonly CommandFactory _commandFactory;
        private readonly IUserResources _userResource;
        private readonly IUserGameDetails _userGameDetails;
        private readonly StatModel _statModel;
        private readonly ReactiveProperty<uint> _coins;
        public ReadOnlyReactiveProperty<string> Name { get; }
        public ReadOnlyReactiveProperty<uint> LevelLabel { get; }
        public ReadOnlyReactiveProperty<ulong> UpgradeValueLabel { get; }
        public ReadOnlyReactiveProperty<ulong> UpgradeCostLabel { get; }
        public ReadOnlyReactiveProperty<bool> IsMaxLevel { get; }
        public uint MaxLevel => _statModel.MaxLevel;

        public StatCardViewModel(ICommandExecutor performer, CommandFactory commandFactory,
            IUserResources userResource, IUserGameDetails userGameDetails, StatModel statModel,
            ReactiveProperty<uint> coins)
        {
            _performer = performer;
            _commandFactory = commandFactory;
            _userResource = userResource;
            _userGameDetails = userGameDetails;
            _statModel = statModel;
            _coins = coins;

            Name = statModel.Name;
            LevelLabel = statModel.Level;
            UpgradeValueLabel = statModel.UpgradeValue;
            UpgradeCostLabel = statModel.UpgradeCost;
            IsMaxLevel = statModel.IsMaxLevel;
        }

        public async void UpgradeStat()
        {
            if (_coins.Value < _statModel.UpgradeCost.CurrentValue || _statModel.IsMaxLevel.CurrentValue) return;
            var costValue = _statModel.UpgradeCost.CurrentValue;
            var status = await _performer.ExecuteCommand(_commandFactory.CreateSpendCoins(_userResource, costValue));
            if (status != Status.Success) return;
            status = await _performer.ExecuteCommand(_commandFactory.CreateUpgradeStat(_statModel, _userGameDetails));
        }
    }
}