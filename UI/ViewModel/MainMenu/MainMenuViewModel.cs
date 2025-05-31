using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Executor.Command;
using Cysharp.Threading.Tasks;
using Model;
using ObservableCollections;
using Proxy;
using R3;
using Router;
using Service;
using UI.Router;
using UI.ViewModel;
using VContainer;
using View;
using Random = UnityEngine.Random;

namespace ViewModel
{
    public class MainMenuViewModel : ViewModelBase<MainMenuModel>
    {
        public readonly ReactiveProperty<uint> EnergyAmount = new();
        public readonly ReactiveProperty<uint> CoinsAmount = new();
        public readonly ReactiveProperty<uint> GemsAmount = new();
        public readonly ReactiveProperty<ulong> LevelAmount = new();

        public readonly ReactiveProperty<string> ActivePage = new();
        public readonly ReactiveProperty<bool> IsShopPageActiveValue = new();
        public readonly ReactiveProperty<bool> IsEquipPageActiveValue = new();
        public readonly ReactiveProperty<bool> IsGamePageActiveValue = new();
        public readonly ReactiveProperty<bool> IsStatsPageActiveValue = new();
        public readonly ReactiveProperty<bool> IsPvPPageActiveValue = new();

        public StatCardViewModel HealthCard;
        public StatCardViewModel AttackCard;
        public StatCardViewModel DefenseCard;

        public HeroViewModel HeroViewModel;
        public InventoryViewModel InventoryViewModel;

        private readonly IRouter<LoadingModel> _loadingRouter;
        private readonly IRouter<PopupEquipmentInfoModel> _popupEquipmentInfoRouter;
        private readonly ICommandExecutor _commandExecutor;
        private readonly ISaveManagement _saveManagement;
        private readonly CommandFactory _commandFactory;
        private readonly ViewModelFactory _viewModelFactory;
        private readonly UserResourceProxy _userResource;
        private readonly IUserContext _userContext;
        private readonly IObjectResolver _resolver;

        [Inject]
        private MainMenuViewModel(IObjectResolver resolver)
        {
            _resolver = resolver;
            _loadingRouter = resolver.Resolve<LoadingRouter>();
            _popupEquipmentInfoRouter = resolver.Resolve<PopupEquipmentInfoRouter>();
            _commandFactory = resolver.Resolve<CommandFactory>();
            _viewModelFactory = resolver.Resolve<ViewModelFactory>();

            _commandExecutor = resolver.Resolve<ICommandExecutor>();
            _saveManagement = resolver.Resolve<ISaveManagement>();
            _userContext = resolver.Resolve<IUserContext>();

            _userResource = new UserResourceProxy(_userContext);
        }

        protected override async UniTask Register()
        {
            await base.Register();

            Model.Energy.Subscribe(x => EnergyAmount.Value = x).AddTo(CompositeDisposable);
            Model.Gems.Subscribe(x => GemsAmount.Value = x).AddTo(CompositeDisposable);
            Model.Coins.Subscribe(x => CoinsAmount.Value = x).AddTo(CompositeDisposable);
            Model.Level.Subscribe(x => LevelAmount.Value = x).AddTo(CompositeDisposable);

            Model.CurrentPage.Subscribe(x => ActivePage.Value = x).AddTo(CompositeDisposable);

            Model.IsShopPageActive.Subscribe(x => IsShopPageActiveValue.Value = x).AddTo(CompositeDisposable);
            Model.IsEquipPageActive.Subscribe(x => IsEquipPageActiveValue.Value = x).AddTo(CompositeDisposable);
            Model.IsGamePageActive.Subscribe(x => IsGamePageActiveValue.Value = x).AddTo(CompositeDisposable);
            Model.IsStatsPageActive.Subscribe(x => IsStatsPageActiveValue.Value = x).AddTo(CompositeDisposable);
            Model.IsPvPPageActive.Subscribe(x => IsPvPPageActiveValue.Value = x).AddTo(CompositeDisposable);

            HealthCard = _viewModelFactory.CreateStatCard(_userResource, Model.HealthStat, Model.Coins);
            AttackCard = _viewModelFactory.CreateStatCard(_userResource, Model.AttackStat, Model.Coins);
            DefenseCard = _viewModelFactory.CreateStatCard(_userResource, Model.DefenseStat, Model.Coins);

            var characterModel = new HeroEquipmentModelProxy(_resolver.Resolve<PlayerEquipmentModel>());

            HeroViewModel = new HeroViewModel(Model.Hero, characterModel);
            InventoryViewModel = new InventoryViewModel(_popupEquipmentInfoRouter, characterModel, Model.Inventory);

            _userResource.Wallet.EnergyRO.Subscribe(x => EnergyAmount.Value = x).AddTo(CompositeDisposable);
            _userResource.Wallet.CoinsRO.Subscribe(x => CoinsAmount.Value = x).AddTo(CompositeDisposable);
            _userResource.Wallet.GemsRO.Subscribe(x => GemsAmount.Value = x).AddTo(CompositeDisposable);

            await _resolver.Resolve<PlayerEquipmentView>().Initialize(characterModel);

            characterModel.ObservableEquipmentSlots.CollectionChanged += HandleEquipmentSlotsChanged;
        }

        private void HandleEquipmentSlotsChanged(in NotifyCollectionChangedEventArgs<KeyValuePair<EquipmentSlot,
            EquipmentModel>> eventArgs)
        {
            if (eventArgs.Action != NotifyCollectionChangedAction.Replace) return;
            if (eventArgs.NewItem.Value != null)
            {
                _userContext.GameState.Set(eventArgs.NewItem.Key.ToString(), eventArgs.NewItem.Value.ID);
            }
            else
            {
                _userContext.GameState.StateData.Remove(eventArgs.OldItem.Key.ToString());
            }
        }

        public override async UniTask Open()
        {
            await _commandExecutor.ExecuteCommand(_commandFactory.CreateInAppPurchase("com.example.product"));
            await _commandExecutor.ExecuteCommand(_commandFactory.CreateAddCoins(_userResource, 50000));
            await _commandExecutor.ExecuteCommand(_commandFactory.CreateOpenUrl("https://google.com"));
            await _saveManagement.AutoSaveGameAsync();
            await _saveManagement.SyncSavesAsync();
            var value = (ulong)Random.Range(15, 50);
            await _commandExecutor.ExecuteCommand(_commandFactory.CreateAddExperience(_userContext, value));
            Model.Experience.Value = Convert.ToUInt64(_userContext.GameState.Get("experience"));
            Model.Level.Value = Convert.ToUInt64(_userContext.GameState.Get("level"));
        }

        public override async UniTask Close()
        {
            _loadingRouter.SetModel(new LoadingModel(new LoadingState("LoadGame", LoadGame)));
            await _loadingRouter.Bind();
        }

        public void ShowPage(string page)
        {
            Model.CurrentPage.Value = page;
        }

        private async UniTask LoadGame()
        {
            var status = await _commandExecutor.ExecuteCommand(_commandFactory.CreateLoadGame());
            CloseCommand.Execute(status == Status.Success);
        }
    }
}