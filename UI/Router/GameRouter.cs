using Cysharp.Threading.Tasks;
using Model;
using UI;
using UI.Router;
using UI.ViewModel;
using VContainer;
using View;
using ViewModel;

namespace Router
{
    public class GameRouter : IRouter<GameModel>
    {
        private readonly IUIManager _uiManager;
        private GameModel _model;
        private IViewModel _viewModel;

        [Inject]
        private GameRouter(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }

        void IRouter<GameModel>.SetModel(GameModel model)
        {
            _model = model;
        }

        async UniTask IRouter.Bind()
        {
            _viewModel = await _uiManager.Bind<GameModel, GameView, GameViewModel>(_model);
            await _viewModel.Open();
            
        }

        UniTask IRouter.Close()
        {
            _viewModel.Close();
            return UniTask.CompletedTask;
        }
    }
}