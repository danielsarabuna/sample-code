using Cysharp.Threading.Tasks;
using FSM;
using Gameplay.Battle;
using Gameplay.Factory;
using VContainer;
using VContainer.Unity;
using View;

namespace Gameplay
{
    public class GameMachine : MachineBase, IInitializable
    {
        private readonly IObjectResolver _resolver;
        private readonly IChallengeRunner _runner;
        // private PlayerEntity _playerEntity;
        private PlayerUnit _playerUnit;

        [Inject]
        private GameMachine(IObjectResolver resolver)
        {
            _resolver = resolver;
            _runner = resolver.Resolve<IChallengeRunner>();
        }

        public void Initialize()
        {
            AddState(new StartState(_resolver));
            AddState(new DialogueState(_resolver));
            AddState(new BattleState(_resolver));
            SetState(StateKey.Start);
        }

        public async UniTask Build()
        {
            // _playerEntity = await _resolver.Resolve<GameEntityFactory>()
            //     .CreatePlayer(_resolver.Resolve<PlayerEntityView>());
            
            _playerUnit = await _resolver.Resolve<UnitFactory>()
                .CreatePlayer(_resolver.Resolve<PlayerEntityView>());
            await _runner.Init();
        }
    }
}