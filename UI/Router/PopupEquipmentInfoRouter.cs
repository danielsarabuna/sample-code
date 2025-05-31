using Cysharp.Threading.Tasks;
using Model;
using UI;
using UI.Router;
using VContainer;
using View;
using ViewModel;

namespace Router
{
    public class PopupEquipmentInfoRouter : IRouter<PopupEquipmentInfoModel>
    {
        private readonly IUIManager _uiManager;
        private PopupEquipmentInfoModel _model;
        private PopupEquipmentInfoViewModel _viewModel;

        [Inject]
        private PopupEquipmentInfoRouter(IUIManager uiManager)
        {
            _uiManager = uiManager;
        }

        void IRouter<PopupEquipmentInfoModel>.SetModel(PopupEquipmentInfoModel model)
        {
            _model = model;
        }

        async UniTask IRouter.Bind()
        {
            _viewModel = await _uiManager.Bind<PopupEquipmentInfoModel, PopupEquipmentInfoView, PopupEquipmentInfoViewModel>(_model);
            await _viewModel.Open();
        }

        UniTask IRouter.Close()
        {
            _viewModel.Close();
            return UniTask.CompletedTask;
        }
    }
}