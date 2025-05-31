using System;
using R3;
using UI.Model;

namespace UI.ViewModel
{
    public abstract class ViewModelBase<TModel> : ViewModel<TModel>, IDisposable
        where TModel : IModel
    {
        public readonly ReactiveCommand<bool> CloseCommand = new();
        protected readonly CompositeDisposable CompositeDisposable = new();

        public virtual void Dispose()
        {
            CompositeDisposable?.Dispose();
        }
    }
}