using System;
using R3;

namespace Model
{
    public class StatModel : IDisposable
    {
        public readonly ReactiveProperty<ulong> UpgradeValue = new BindableReactiveProperty<ulong>();
        public readonly ReactiveProperty<ulong> CurrentValue = new BindableReactiveProperty<ulong>();
        public readonly ReactiveProperty<ulong> UpgradeCost = new BindableReactiveProperty<ulong>();
        public readonly ReactiveProperty<uint> Level = new ReactiveProperty<uint>();
        public readonly ReactiveProperty<string> Name;
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly ReactiveProperty<bool> _isMaxLevel;
        public ReadOnlyReactiveProperty<bool> IsMaxLevel => _isMaxLevel;
        public uint MaxLevel { get; }

        public StatModel(string name, uint maxLevel)
        {
            Name = new ReactiveProperty<string>(name);
            _isMaxLevel = new ReactiveProperty<bool>(false);
            MaxLevel = maxLevel;
            Level.Subscribe(x => { _isMaxLevel.Value = x >= MaxLevel; }).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}