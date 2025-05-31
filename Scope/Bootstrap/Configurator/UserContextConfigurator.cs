using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Executor.Command;
using Cysharp.Threading.Tasks;
using Gameplay;
using Model;
using Service;
using VContainer;

public partial class Bootstrap
{
    private class UserContextIConfigurator : IConfigurator, ITaskScheduler
    {
        void IConfigurator.Configure(IContainerBuilder builder)
        {
            var playerEquipmentModel = new PlayerEquipmentModel("Kaganobu", 1); // TODO: temp
            builder.RegisterInstance(playerEquipmentModel);
            builder.Register<IUserContext, UserContextModifier>(Lifetime.Singleton) // TODO: UserContextModifier
                .As<IUserLocalization>()
                .As<IUserSession>()
                .As<IUserPermissions>()
                .As<IUserGameDetails>()
                .As<IUserResources>()
                .As<IUserInventory>()
                .As<IUserIdentity>()
                .As<IUserActivityLogging>()
                .As<IUserContextModifier>();
        }

        IList<LoadingState> ITaskScheduler.CreateTaskList(IObjectResolver builder, CancellationToken cancellation)
        {
            var userGameDetails = builder.Resolve<IUserGameDetails>();
            var commandExecutor = builder.Resolve<ICommandExecutor>();
            var commandFactory = builder.Resolve<CommandFactory>();
            var list = new List<LoadingState>
            {
                new("SetSettings", () => SetSettings(builder)),
                new("LoadPlayerEquipment", () => LoadPlayerEquipment(builder))
            };

            switch (userGameDetails.GameState.CurrentScene)
            {
                case "Game":
                    list.Add(new LoadingState("LoadGame", () => LoadGame(commandExecutor, commandFactory))); break;
                default:
                    list.Add(new LoadingState("LoadMainMenu", () => LoadMainMenu(commandExecutor, commandFactory)));
                    break;
            }

            return list;
        }

        private async UniTask SetSettings(IObjectResolver builder)
        {
            var userLocalization = builder.Resolve<IUserLocalization>();
            var commandFactory = builder.Resolve<CommandFactory>();
            var commandExecutor = builder.Resolve<ICommandExecutor>();
            var graphicCommand = commandFactory.CreateSetGraphic(userLocalization.Settings.FrameRate,
                userLocalization.Settings.Graphic);
            await commandExecutor.ExecuteCommand(graphicCommand);
        }

        private async UniTask LoadGame(ICommandExecutor commandExecutor, CommandFactory commandFactory)
        {
            var status = await commandExecutor.ExecuteCommand(commandFactory.CreateLoadGame());
        }

        private async UniTask LoadMainMenu(ICommandExecutor commandExecutor, CommandFactory commandFactory)
        {
            var status = await commandExecutor.ExecuteCommand(commandFactory.CreateLoadMainMenu());
        }

        private UniTask LoadPlayerEquipment(IObjectResolver resolver)
        {
            var equipmentModel = resolver.Resolve<PlayerEquipmentModel>();
            var userGameDetails = resolver.Resolve<IUserGameDetails>();
            var equipmentCollection = resolver.Resolve<EquipmentCollection>();

            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                if (!userGameDetails.GameState.StateData.TryGetValue(slot.ToString(), out var s)) continue;
                var id = s.ToString();
                var model = equipmentCollection.Models.FirstOrDefault(x => x.ID == id);
                if (model == null) continue;
                equipmentModel.Equip(model, slot);
            }

            return UniTask.CompletedTask;
        }
    }
}