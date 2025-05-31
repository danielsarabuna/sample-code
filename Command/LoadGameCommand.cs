using Common;
using Cysharp.Threading.Tasks;
using Service;
using UnityEngine.SceneManagement;

namespace Executor.Command
{
    public class LoadGameCommand : ICommand
    {
        private readonly ISceneManagement _scene;
        private readonly ApplicationManager _applicationManager;

        public LoadGameCommand(ISceneManagement scene, ApplicationManager applicationManager)
        {
            _scene = scene;
            _applicationManager = applicationManager;
        }

        public async UniTask<Status> Execute()
        {
            await _scene.LoadSceneAsync("Boot", LoadSceneMode.Single);
            await _scene.LoadSceneAsync("Game", LoadSceneMode.Single);
            await UniTask.WaitUntil(() => _applicationManager.CurrentState == ApplicationState.Game);
            return await UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}