using Cysharp.Threading.Tasks;
using Model;
using Service;

namespace Executor.Command
{
    public class SpendCoinsCommand : ICommand
    {
        private readonly IDebugService _debugService;
        private readonly IUserResources _resourceProxy;
        private readonly uint _value;

        public SpendCoinsCommand(IDebugService debugService, IUserResources resourceProxy, uint value)
        {
            _debugService = debugService;
            _resourceProxy = resourceProxy;
            _value = value;
        }

        public UniTask<Status> Execute()
        {
            _debugService.Log($"Execute started. Current Coins: {_resourceProxy.Wallet.Coins}, Value to spend: {_value}");
            _resourceProxy.Wallet.SpendCoins(_value);
            _debugService.Log($"Coins updated. New Coins: {_resourceProxy.Wallet.Coins}");
            return UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            return UniTask.FromResult(Status.Success);
        }
    }
}