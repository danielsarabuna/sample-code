using System;
using Common;
using Cysharp.Threading.Tasks;
using Gameplay;
using Gameplay.Battle;
using Gameplay.Factory;
using Model;
using Root.Game;
using Router;
using Service;
using UI.Router;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;
using ViewModel;

public class Game : GameStateScope
{
    [SerializeField] private Camera _camera;
    [SerializeField] private TextAsset _textAsset;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private PlayerEntityView _playerEntityView;
    [SerializeField] private PlayerEquipmentView _playerEquipmentView;
    private ProgressionService _progression;
    private ISaveManagement _saveManagement;
    private IUserContext _userContext;

    protected override ApplicationState State => ApplicationState.Game;

    protected override void Resolve(IObjectResolver resolver)
    {
        base.Resolve(resolver);
        _saveManagement = resolver.Resolve<ISaveManagement>();
        _userContext = resolver.Resolve<IUserContext>();
        _progression = resolver.Resolve<ProgressionService>();
    }

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.Register<GameRouter>(Lifetime.Transient);
        builder.Register<GameViewModel>(Lifetime.Transient);
        builder.Register<GameView>(Lifetime.Transient);

        builder.Register<StoryViewModel>(Lifetime.Singleton);
        builder.Register<StoryModel>(Lifetime.Singleton);

        builder.Register<IChallengeRunner, ChallengeRunner>(Lifetime.Singleton)
            .WithParameter("json", _textAsset.text);

        builder.Register<IEventChoiceHandler, EventChoiceHandler>(Lifetime.Singleton);
        builder.Register<IEventSystem, EventSystem>(Lifetime.Singleton)
            .WithParameter("maxHistorySize", 3);

        builder.RegisterEntryPoint<AutoBattleSystem>(Lifetime.Scoped);
        builder.Register<CombatSystem>(Lifetime.Scoped);
        builder.Register<UnitFactory>(Lifetime.Scoped);
        builder.Register<AbilityFactory>(Lifetime.Scoped);

        builder.Register<IBattleStateController, BattleStateController>(Lifetime.Singleton);

        builder.RegisterEntryPoint<GameMachine>().AsSelf();

        builder.Register<ActorModelFactory>(Lifetime.Singleton);
        builder.RegisterComponent(_camera);
        builder.RegisterComponent(_spawnManager);
        builder.RegisterComponent(_playerEntityView);
        builder.RegisterComponent(_playerEquipmentView);
    }

    protected override async UniTask Initialize()
    {
        IRouter<GameModel> router = Container.Resolve<GameRouter>();
        var model = BuildModel();
        router.SetModel(model);
        await router.Bind();
        await Container.Resolve<GameMachine>().Build();
        _userContext.GameState.CurrentScene = "Game";
        await _saveManagement.AutoSaveGameAsync();
        await _saveManagement.SyncSavesAsync();
    }

    private GameModel BuildModel()
    {
        var model = new GameModel();
        model.Energy.Value = _userContext.Wallet.Energy;
        model.Gems.Value = _userContext.Wallet.Gems;
        model.Coins.Value = _userContext.Wallet.Coins;

        var stateData = _userContext.GameState.StateData;
        model.HeroName.Value = Convert.ToString(stateData["hero-name"]);
        model.Level.Value = Convert.ToUInt64(stateData["level"]);
        model.Experience.Value = Convert.ToUInt64(stateData["experience"]);

        var healthLevel = Convert.ToUInt32(stateData["health"]);
        var attackLevel = Convert.ToUInt32(stateData["attack"]);
        var defenseLevel = Convert.ToUInt32(stateData["defense"]);

        model.Health.Value = CalculateStatCurrentValue("health", healthLevel);
        model.Attack.Value = CalculateStatCurrentValue("attack", attackLevel);
        model.Defense.Value = CalculateStatCurrentValue("defense", defenseLevel);

        model.BattleSpeed.Value = $"X{Container.Resolve<Common.IBattleStateController>().CurrentTimeScale,0}";

        return model;
    }

    private ulong CalculateStatCurrentValue(string stat, uint level)
    {
        return _progression.Get<IProgressionCalculator>($"{stat}-value").GetValue(level);
    }
}