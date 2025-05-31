using Cysharp.Threading.Tasks;

namespace Executor.Command
{
    public interface ICommandExecutor
    {
        UniTask<Status> ExecuteCommand(ICommand command);
        UniTask<Status> UndoCommand();
        UniTask<Status> RedoCommand();
    }
}