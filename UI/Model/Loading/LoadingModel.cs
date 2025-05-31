using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UI.Model;

namespace Model
{
    public sealed class LoadingModel : IModel
    {
        public ReadOnlyCollection<LoadingState> States { get; }
        public float Delay { get; } = 0F;

        public LoadingModel(LoadingState loadingState)
        {
            States = new ReadOnlyCollection<LoadingState>(new[] { loadingState });
        }

        public LoadingModel(IList<LoadingState> states)
        {
            States = new ReadOnlyCollection<LoadingState>(states);
        }
        
        public LoadingModel(IList<LoadingState> states, float delay)
        {
            States = new ReadOnlyCollection<LoadingState>(states);
            Delay = delay;
        }

        void IDisposable.Dispose()
        {
        }
    }
}