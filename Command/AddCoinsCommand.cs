using Cysharp.Threading.Tasks;
using Model;
using Service;

namespace Executor.Command
{
    public record AddCoinsCommand : ICommand
    {
        private readonly IDebugService _debugService;
        private readonly IUserResources _resourceProxy;
        private readonly uint _value;

        public AddCoinsCommand(IDebugService debugService, IUserResources resourceProxy, uint value)
        {
            _debugService = debugService;
            _resourceProxy = resourceProxy;
            _value = value;
        }

        public UniTask<Status> Execute()
        {
            _debugService.Log($"Execute started. Current Coins: {_resourceProxy.Wallet.Coins}, Value to add: {_value}");
            _resourceProxy.Wallet.AddCoins(_value);
            _debugService.Log($"Coins updated. New Coins: {_resourceProxy.Wallet.Coins}");
            return UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            return UniTask.FromResult(Status.Success);
        }
    }
}