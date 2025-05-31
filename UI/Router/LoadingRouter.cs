using Cysharp.Threading.Tasks;
using Model;
using UI;
using UI.Router;
using VContainer;
using View;
using ViewModel;

namespace Router
{
    public class LoadingRouter : IRouter<LoadingModel>
    {
        private readonly IUIManager _uiManager;
        private LoadingModel _model;
        private LoadingViewModel _viewModel;

        [Inject]
        private LoadingRouter(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }

        void IRouter<LoadingModel>.SetModel(LoadingModel model)
        {
            _model = model;
        }

        async UniTask IRouter.Bind()
        {
            _viewModel = await _uiManager.Bind<LoadingModel, LoadingView, LoadingViewModel>(_model);
            await _viewModel.Open();
            _viewModel.Close();
        }

        UniTask IRouter.Close()
        {
            _viewModel.Close();
            return UniTask.CompletedTask;
        }
    }
}