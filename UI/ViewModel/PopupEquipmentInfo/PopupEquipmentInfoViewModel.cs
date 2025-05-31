using Cysharp.Threading.Tasks;
using Executor.Command;
using Model;
using ObservableCollections;
using R3;
using Service;
using UI.ViewModel;
using VContainer;

namespace ViewModel
{
    public class PopupEquipmentInfoViewModel : ViewModelBase<PopupEquipmentInfoModel>
    {
        private readonly ReactiveProperty<string> _nameLabel = new ReactiveProperty<string>();
        private readonly ReactiveProperty<string> _descriptionLabel = new ReactiveProperty<string>();
        private readonly ReactiveProperty<ulong> _upgradeCost = new ReactiveProperty<ulong>();
        private readonly ReactiveProperty<bool> _equipButonActive = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _takeOffButtonActive = new ReactiveProperty<bool>();
        private readonly ICommandExecutor _commandExecutor;
        private readonly ISaveManagement _saveManagement;
        private readonly CommandFactory _commandFactory;
        public ReadOnlyReactiveProperty<string> NameLabel => _nameLabel;
        public ReadOnlyReactiveProperty<string> DescriptionLabel => _descriptionLabel;
        public IReadOnlyObservableDictionary<string, double> Properties { get; private set; }
        public IReadOnlyObservableList<string> Effects { get; private set; }
        public IReadOnlyObservableList<string> Ability { get; private set; }
        public ReadOnlyReactiveProperty<ulong> UpgradeCostLabel => _upgradeCost;
        public ReadOnlyReactiveProperty<bool> EquipButonActive => _equipButonActive;
        public ReadOnlyReactiveProperty<bool> TakeOffButtonActive => _takeOffButtonActive;
        public EquipmentModel Equipment => Model.Equipment;

        [Inject]
        private PopupEquipmentInfoViewModel(IObjectResolver resolver)
        {
            _commandFactory = resolver.Resolve<CommandFactory>();
            _commandExecutor = resolver.Resolve<ICommandExecutor>();
            _saveManagement = resolver.Resolve<ISaveManagement>();
        }

        protected override async UniTask Register()
        {
            await base.Register();
            _nameLabel.Value = Model.Name;
            _descriptionLabel.Value = Model.Description;
            Properties = Model.Properties;
            Effects = Model.Effects;
            Ability = Model.Ability;
            _equipButonActive.Value = !Model.CharacterModel.IsEquipped(Model.Equipment);
            _takeOffButtonActive.Value = !_equipButonActive.Value;
            Model.UpgradeCost.Subscribe(value => _upgradeCost.Value = value).AddTo(CompositeDisposable);
        }

        public override UniTask Open()
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Close()
        {
            CloseCommand.Execute(true);
            return UniTask.CompletedTask;
        }

        public void Upgrade()
        {
            throw new System.NotImplementedException();
        }

        public async void Equip()
        {
            var command = _commandFactory.CreateEquip(Model.CharacterModel, Model.Equipment);
            var status = await _commandExecutor.ExecuteCommand(command);
            if (status != Status.Success) return;
            await Close();
            await _saveManagement.AutoSaveGameAsync();
            await _saveManagement.SyncSavesAsync();
        }
        
        public async void TakeOff()
        {
            var command = _commandFactory.CreateTakeOff(Model.CharacterModel, Model.Equipment);
            var status = await _commandExecutor.ExecuteCommand(command);
            if (status != Status.Success) return;
            await Close();
            await _saveManagement.AutoSaveGameAsync();
            await _saveManagement.SyncSavesAsync();
        }
    }
}