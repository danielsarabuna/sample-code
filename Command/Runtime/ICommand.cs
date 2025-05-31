using Cysharp.Threading.Tasks;

namespace Executor.Command
{
    public enum Status
    {
        Success,
        Failure
    }

    public interface ICommand
    {
        UniTask<Status> Execute();
        UniTask<Status> Undo();
    }
}