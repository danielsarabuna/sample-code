using Executor.Command;
using Model;
using R3;
using UI;
using VContainer;

namespace ViewModel
{
    public class ViewModelFactory
    {
        private readonly ICommandExecutor _performer;
        private readonly CommandFactory _commandFactory;
        private readonly IUserContext _userContext;

        [Inject]
        private ViewModelFactory(IObjectResolver resolver)
        {
            _userContext = resolver.Resolve<IUserContext>();
            _commandFactory = resolver.Resolve<CommandFactory>();
            _performer = resolver.Resolve<ICommandExecutor>();
        }

        public StatCardViewModel CreateStatCard(IUserResources userResource, StatModel statModel,
            ReactiveProperty<uint> property)
        {
            return new StatCardViewModel(_performer, _commandFactory, userResource, _userContext, statModel, property);
        }
    }
}