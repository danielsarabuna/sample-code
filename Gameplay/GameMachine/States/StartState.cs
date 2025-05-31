using FSM;
using VContainer;

namespace Gameplay
{
    public class StartState : IState, IEnter
    {
        private readonly IObjectResolver _resolver;
        private readonly GameMachine _gameMachine;

        public bool CanSwitch(IState state)
        {
            return true;
        }

        byte IState.ID => StateKey.Start;

        public StartState(IObjectResolver resolver)
        {
            _resolver = resolver;
            _gameMachine = resolver.Resolve<GameMachine>();
        }

        void IEnter.Enter()
        {
            _gameMachine.SetState(StateKey.Dialogue);
        }
    }
}