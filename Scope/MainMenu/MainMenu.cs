using System;
using Cysharp.Threading.Tasks;
using Gameplay;
using Model;
using Router;
using Service;
using UI.Router;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;
using Common;
using ViewModel;

public class MainMenu : GameStateScope
{
    [SerializeField] private PreviewHeroEquipmentView _previewHeroEquipmentView;
    [SerializeField] private PlayerEquipmentView _playerEquipmentView;
    private ISaveManagement _saveManagement;
    private IUserContext _userContext;
    private ProgressionService _progression;
    private EquipmentCollection _equipmentCollection;

    protected override ApplicationState State => ApplicationState.MainMenu;

    protected override void Resolve(IObjectResolver resolver)
    {
        base.Resolve(resolver);
        _saveManagement = resolver.Resolve<ISaveManagement>();
        _progression = resolver.Resolve<ProgressionService>();
        _userContext = resolver.Resolve<IUserContext>();
        _equipmentCollection = resolver.Resolve<EquipmentCollection>();
    }

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);

        builder.Register<MainMenuRouter>(Lifetime.Transient);
        builder.Register<MainMenuViewModel>(Lifetime.Transient);
        builder.Register<MainMenuView>(Lifetime.Transient);
        builder.Register<MainMenuModel>(Lifetime.Singleton);

        builder.Register<PopupEquipmentInfoRouter>(Lifetime.Transient);
        builder.Register<PopupEquipmentInfoViewModel>(Lifetime.Transient);
        builder.Register<PopupEquipmentInfoView>(Lifetime.Transient);

        builder.Register<ViewModelFactory>(Lifetime.Singleton);

        builder.RegisterComponent(_previewHeroEquipmentView);
        builder.RegisterComponent(_playerEquipmentView);
    }

    protected override async UniTask Initialize()
    {
        await Container.Resolve<IAssetManagement>().LoadAssetsByLabelAsync<Texture2D>("Equipment");
        IRouter<MainMenuModel> router = Container.Resolve<MainMenuRouter>();
        var model = BuildModel();
        router.SetModel(model);
        await router.Bind();
        _userContext.GameState.CurrentScene = "MainMenu";
        await _saveManagement.AutoSaveGameAsync();
        await _saveManagement.SyncSavesAsync();
    }

    private MainMenuModel BuildModel()
    {
        var model = new MainMenuModel();
        model.Energy.Value = _userContext.Wallet.Energy;
        model.Gems.Value = _userContext.Wallet.Gems;
        model.Coins.Value = _userContext.Wallet.Coins;

        var stateData = _userContext.GameState.StateData;
        model.HeroName.Value = Convert.ToString(stateData["hero-name"]);
        model.Level.Value = Convert.ToUInt64(stateData["level"]);
        model.Experience.Value = Convert.ToUInt64(stateData["experience"]);

        model.CurrentPage.Value = "Game";

        model.IsShopPageActive.Value = false;
        model.IsEquipPageActive.Value = true;
        model.IsGamePageActive.Value = true;
        model.IsStatsPageActive.Value = true;
        model.IsPvPPageActive.Value = false;

        model.HealthStat.Level.Value = Convert.ToUInt32(stateData["health"]);
        model.AttackStat.Level.Value = Convert.ToUInt32(stateData["attack"]);
        model.DefenseStat.Level.Value = Convert.ToUInt32(stateData["defense"]);

        model.HealthStat.CurrentValue.Value = CalculateStatCurrentValue("health", model.HealthStat.Level.Value);
        model.HealthStat.UpgradeValue.Value = CalculateStatUpgradeValue("health", model.HealthStat.Level.Value);
        model.HealthStat.UpgradeCost.Value = CalculateStatUpgradeCost("health", model.HealthStat.Level.Value);

        model.AttackStat.CurrentValue.Value = CalculateStatCurrentValue("attack", model.HealthStat.Level.Value);
        model.AttackStat.UpgradeValue.Value = CalculateStatUpgradeValue("attack", model.HealthStat.Level.Value);
        model.AttackStat.UpgradeCost.Value = CalculateStatUpgradeCost("attack", model.HealthStat.Level.Value);

        model.DefenseStat.CurrentValue.Value = CalculateStatCurrentValue("defense", model.HealthStat.Level.Value);
        model.DefenseStat.UpgradeValue.Value = CalculateStatUpgradeValue("defense", model.HealthStat.Level.Value);
        model.DefenseStat.UpgradeCost.Value = CalculateStatUpgradeCost("defense", model.HealthStat.Level.Value);

        model.Hero.HeroName = model.HeroName;
        model.Hero.Level = model.Level;
        model.Hero.Experience = model.Experience;

        model.Hero.Health = model.HealthStat.CurrentValue;
        model.Hero.Attack = model.AttackStat.CurrentValue;
        model.Hero.Defense = model.DefenseStat.CurrentValue;

#if UNITY_EDITOR
        foreach (var equipmentModel in _equipmentCollection.Models) // TODO: load inventory from user
        {
            if (equipmentModel is not ArtifactModel)
                model.Inventory.AddItem(equipmentModel);
        }
#endif
        return model;
    }

    private ulong CalculateStatCurrentValue(string stat, uint level)
    {
        return _progression.Get<IProgressionCalculator>($"{stat}-value").GetValue(level);
    }

    private ulong CalculateStatUpgradeValue(string stat, uint level)
    {
        var statCurrentValue = CalculateStatCurrentValue(stat, level);
        return _progression.Get<IProgressionCalculator>($"{stat}-value").GetValue(level + 1) - statCurrentValue;
    }

    private ulong CalculateStatUpgradeCost(string stat, uint level)
    {
        return _progression.Get<IProgressionCalculator>($"{stat}-cost").GetValue(level);
    }
}