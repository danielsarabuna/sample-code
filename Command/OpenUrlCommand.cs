using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Executor.Command
{
    public record OpenUrlCommand : ICommand
    {
        private readonly string _url;

        public OpenUrlCommand(string url)
        {
            _url = url;
        }

        public UniTask<Status> Execute()
        {
            Debug.Log($"Execute started. Opening URL: {_url}");
#if !UNITY_EDITOR
            Application.OpenURL(_url);
#endif
            return UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            return UniTask.FromResult(Status.Success);
        }
    }
}