namespace Common
{
    public enum ApplicationState
    {
        Boot,
        MainMenu,
        Game
    }

    public interface IStateChangeable
    {
        ApplicationState State { set; }
    }

    public class ApplicationManager : IStateChangeable
    {
        public ApplicationState CurrentState { get; private set; } = ApplicationState.Boot;

        public ApplicationState State
        {
            set => CurrentState = value;
        }
    }
}