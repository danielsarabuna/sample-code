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
    public class MainMenuRouter : IRouter<MainMenuModel>
    {
        private readonly IUIManager _uiManager;
        private MainMenuModel _model;
        private IViewModel _viewModel;

        [Inject]
        private MainMenuRouter(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }

        void IRouter<MainMenuModel>.SetModel(MainMenuModel model)
        {
            _model = model;
        }

        async UniTask IRouter.Bind()
        {
            _viewModel = await _uiManager.Bind<MainMenuModel, MainMenuView, MainMenuViewModel>(_model);
            await _viewModel.Open();
        }

        UniTask IRouter.Close()
        {
            _viewModel.Close();
            return UniTask.CompletedTask;
        }
    }
}