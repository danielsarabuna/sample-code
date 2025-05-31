using Common;
using Cysharp.Threading.Tasks;
using Model;
using Service;
using UnityEngine.SceneManagement;

namespace Executor.Command
{
    public class LoadMainMenuCommand : ICommand
    {
        private readonly ISceneManagement _scene;
        private readonly ApplicationManager _applicationManager;

        public LoadMainMenuCommand(ISceneManagement scene, ApplicationManager applicationManager)
        {
            _scene = scene;
            _applicationManager = applicationManager;
        }

        public async UniTask<Status> Execute()
        {
            await _scene.LoadSceneAsync("Boot", LoadSceneMode.Single);
            await _scene.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
            await UniTask.WaitUntil(() => _applicationManager.CurrentState == ApplicationState.MainMenu);
            return await UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}