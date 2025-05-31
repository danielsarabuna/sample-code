using Common;
using Cysharp.Threading.Tasks;
using UI;
using VContainer;
using VContainer.Unity;

public abstract class GameStateScope : LifetimeScope
{
    protected abstract ApplicationState State { get; }
    [Inject] private IStateChangeable _stateChangeable;
    [Inject] private IUIManager _uiManager;

    private void Reset()
    {
        autoRun = false;
    }

    protected override void Awake()
    {
#if UNITY_EDITOR
        if (gameObject.TryGetComponent(out UnityEngine.UIElements.UIDocument uiDocument))
            uiDocument.enabled = false;
#endif
        base.Awake();
        if (Parent == null) return;
        Resolve(Parent.Container);
    }


    protected virtual void Resolve(IObjectResolver resolver)
    {
        name = "Temp";
        resolver.Inject(this);
    }

    private async void Start()
    {
        Build();
        _uiManager.Resolver = Container;
        await Initialize();
        _stateChangeable.State = State;
    }

    protected abstract UniTask Initialize();
}