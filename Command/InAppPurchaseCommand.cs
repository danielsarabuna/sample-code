using Cysharp.Threading.Tasks;
using Model;
using Service;

namespace Executor.Command
{
    public class InAppPurchaseCommand : ICommand
    {
        private readonly IDebugService _debug;
        private readonly IUserContext _userContext;
        private readonly string _productId;

        public InAppPurchaseCommand(IDebugService debug, IUserContext userContext, string productId)
        {
            _debug = debug;
            _userContext = userContext;
            _productId = productId;
        }

        public UniTask<Status> Execute()
        {
            _debug.Log($"{nameof(InAppPurchaseCommand)}: {_productId}");
            return UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            return UniTask.FromResult(Status.Success);
        }
    }
}