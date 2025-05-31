using Cysharp.Threading.Tasks;
using Model;
using Service;
using UnityEngine;
using VContainer;

namespace View
{
    public abstract class CharacterViewBase : MonoBehaviour
    {
        [SerializeField] protected Transform _root, _visual;
    }
    
    public abstract class CharacterViewBase<TModel> : CharacterViewBase where TModel : CharacterModel
    {
        protected IAssetManagement AssetManagement;
        protected TModel Model { get; private set; }

        protected virtual void Reset()
        {
            _root = transform.GetChild(0);
            _visual = _root.GetChild(0);
        }

        [Inject]
        protected virtual void Construct(IObjectResolver resolver)
        {
            AssetManagement = resolver.Resolve<IAssetManagement>();
        }

        public virtual UniTask Initialize(TModel model)
        {
            Model = model;
            return UniTask.CompletedTask;
        }

        public UniTask Show()
        {
            _visual.gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public UniTask Hide()
        {
            _visual.gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
    }
}