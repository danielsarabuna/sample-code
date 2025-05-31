using System;
using Common;
using Model;
using Service;
using VContainer;

namespace Executor.Command
{
    public class CommandFactory
    {
        private readonly IUserContext _userContext;
        private readonly IDebugService _debugService;
        private readonly ISceneManagement _sceneManagement;
        private readonly ProgressionService _progressionService;
        private readonly ApplicationManager _applicationManager;

        public CommandFactory(IObjectResolver resolver)
        {
            _userContext = resolver.Resolve<IUserContext>();
            _debugService = resolver.Resolve<IDebugService>();
            _sceneManagement = resolver.Resolve<ISceneManagement>();
            _progressionService = resolver.Resolve<ProgressionService>();
            _applicationManager = resolver.Resolve<ApplicationManager>();
        }

        public ICommand CreateAddCoins(IUserResources resources, uint value)
        {
            return new AddCoinsCommand(_debugService, resources, value);
        }

        public ICommand CreateSpendCoins(IUserResources resources, ulong value)
        {
            return new SpendCoinsCommand(_debugService, resources, Convert.ToUInt32(value));
        }

        public ICommand CreateOpenUrl(string url)
        {
            return new OpenUrlCommand(url);
        }

        public ICommand CreateSetGraphic(short frameRate, short graphic)
        {
            return new SetGraphicCommand(frameRate, graphic);
        }

        public ICommand CreateAddExperience(IUserGameDetails userGameDetails, ulong value)
        {
            return new AddExperienceCommand(_debugService, userGameDetails, _progressionService, value);
        }

        public ICommand CreateLoadMainMenu()
        {
            return new LoadMainMenuCommand(_sceneManagement, _applicationManager);
        }

        public ICommand CreateLoadGame()
        {
            return new LoadGameCommand(_sceneManagement, _applicationManager);
        }

        public ICommand CreateInAppPurchase(string productId)
        {
            return new InAppPurchaseCommand(_debugService, _userContext, productId);
        }

        public ICommand CreateUpgradeStat(StatModel statModel, IUserGameDetails userGameDetails)
        {
            var key = statModel.Name.CurrentValue.ToLower();
            var calculatorValue = _progressionService.Get<IProgressionCalculator>($"{key}-value");
            var calculatorCost = _progressionService.Get<IProgressionCalculator>($"{key}-cost");
            return new UpgradeStatCommand(_debugService, userGameDetails, statModel, calculatorValue, calculatorCost);
        }

        public ICommand CreateEquip(CharacterModel characterModel, EquipmentModel equipment)
        {
            return new EquipCommand(characterModel, equipment);
        }
        
        public ICommand CreateTakeOff(CharacterModel characterModel, EquipmentModel equipment)
        {
            return new TakeOffCommand(characterModel, equipment);
        }
    }
}