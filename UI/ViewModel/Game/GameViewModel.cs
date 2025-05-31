using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Executor.Command;
using Model;
using Proxy;
using R3;
using Router;
using UI.Router;
using UI.ViewModel;
using VContainer;
using View;

namespace ViewModel
{
    public class GameViewModel : ViewModelBase<GameModel>
    {
        public readonly ReactiveProperty<uint> EnergyAmount = new();
        public readonly ReactiveProperty<uint> CoinsAmount = new();
        public readonly ReactiveProperty<uint> GemsAmount = new();

        public readonly ReactiveProperty<ulong> Level = new();
        public readonly ReactiveProperty<ulong> Experience = new();
        public readonly ReactiveProperty<ulong> HealthAmount = new();
        public readonly ReactiveProperty<ulong> AttackAmount = new();
        public readonly ReactiveProperty<ulong> DefenseAmount = new();
        public readonly ReactiveProperty<string> BattleSpeedLabel = new();

        public readonly StoryViewModel StoryViewModel;
        private readonly IRouter<LoadingModel> _loadingRouter;
        private readonly ICommandExecutor _commandExecutor;
        private readonly CommandFactory _commandFactory;
        private readonly IObjectResolver _resolver;
        private readonly IUserContext _userContext;
        private readonly UserResourceProxy _userResource;
        private readonly IBattleStateController _battleStateController;

        [Inject]
        private GameViewModel(IObjectResolver resolver)
        {
            _resolver = resolver;
            _loadingRouter = resolver.Resolve<LoadingRouter>();
            StoryViewModel = resolver.Resolve<StoryViewModel>();
            _commandExecutor = resolver.Resolve<ICommandExecutor>();
            _commandFactory = resolver.Resolve<CommandFactory>();
            _userContext = resolver.Resolve<IUserContext>();
            _battleStateController = resolver.Resolve<IBattleStateController>();

            _userResource = new UserResourceProxy(_userContext);
        }

        protected override async UniTask Register()
        {
            await base.Register();

            Model.Energy.Subscribe(x => EnergyAmount.Value = x).AddTo(CompositeDisposable);
            Model.Gems.Subscribe(x => GemsAmount.Value = x).AddTo(CompositeDisposable);
            Model.Coins.Subscribe(x => CoinsAmount.Value = x).AddTo(CompositeDisposable);

            Model.Level.Subscribe(x => Level.Value = x).AddTo(CompositeDisposable);
            Model.Experience.Subscribe(x => Experience.Value = x).AddTo(CompositeDisposable);
            Model.Health.Subscribe(x => HealthAmount.Value = x).AddTo(CompositeDisposable);
            Model.Attack.Subscribe(x => AttackAmount.Value = x).AddTo(CompositeDisposable);
            Model.Defense.Subscribe(x => DefenseAmount.Value = x).AddTo(CompositeDisposable);
            Model.BattleSpeed.Subscribe(x => BattleSpeedLabel.Value = x).AddTo(CompositeDisposable);

            _userResource.Wallet.EnergyRO.Subscribe(x => EnergyAmount.Value = x).AddTo(CompositeDisposable);
            _userResource.Wallet.CoinsRO.Subscribe(x => CoinsAmount.Value = x).AddTo(CompositeDisposable);
            _userResource.Wallet.GemsRO.Subscribe(x => GemsAmount.Value = x).AddTo(CompositeDisposable);


            var characterModel = new HeroEquipmentModelProxy(_resolver.Resolve<PlayerEquipmentModel>());
            await _resolver.Resolve<PlayerEquipmentView>().Initialize(characterModel);
        }

        public override async UniTask Open()
        {
            StoryViewModel.Continue();
            StoryViewModel.Continue();
            await UniTask.CompletedTask;
        }

        public override async UniTask Close()
        {
            _loadingRouter.SetModel(new LoadingModel(new LoadingState("LoadMainMenu", LoadMainMenu)));
            await _loadingRouter.Bind();
        }

        private async UniTask LoadMainMenu()
        {
            var status = await _commandExecutor.ExecuteCommand(_commandFactory.CreateLoadMainMenu());
            CloseCommand.Execute(status == Status.Success);
        }

        public void BattleSpeedUp()
        {
            _battleStateController.SetBattleSpeedUp();
            BattleSpeedLabel.Value = $"X{_battleStateController.CurrentTimeScale,0}";
        }
    }
}