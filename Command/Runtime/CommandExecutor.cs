using System.Collections.Generic;
using Executor.Command;
using Cysharp.Threading.Tasks;

namespace Executor.Data
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly Stack<ICommand> _undoes = new();
        private readonly Stack<ICommand> _redoes = new();

        public async UniTask<Status> ExecuteCommand(ICommand command)
        {
            var status = await command.Execute();
            _undoes.Push(command);
            return await UniTask.FromResult(status);
        }

        public async UniTask<Status> UndoCommand()
        {
            if (_undoes.Count <= 0) return Status.Failure;
            var command = _undoes.Pop();
            _redoes.Push(command);
            return await command.Undo();
        }

        public async UniTask<Executor.Command.Status> RedoCommand()
        {
            if (_redoes.Count <= 0) return Status.Failure;
            var command = _redoes.Pop();
            return await ExecuteCommand(command);
        }
    }
}