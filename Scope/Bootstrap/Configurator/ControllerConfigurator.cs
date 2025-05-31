using Executor.Command;
using Executor.Data;
using VContainer;

public partial class Bootstrap
{
    public class ControllerIConfigurator : IConfigurator
    {
        void IConfigurator.Configure(IContainerBuilder builder)
        {
            builder.Register<CommandFactory>(Lifetime.Singleton);
            builder.Register<ICommandExecutor, CommandExecutor>(Lifetime.Singleton);
        }
    }
}