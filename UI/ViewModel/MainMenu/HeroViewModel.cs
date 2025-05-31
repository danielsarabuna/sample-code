using System;
using Model;
using Proxy;
using R3;

namespace ViewModel
{
    public class HeroViewModel : IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly HeroModel _model;
        public HeroEquipmentModelProxy HeroEquipment;

        public ReadOnlyReactiveProperty<string> HeroNameValue { get; }
        public ReadOnlyReactiveProperty<ulong> LevelAmount { get; }
        public ReadOnlyReactiveProperty<ulong> ExperienceAmount { get; }
        public ReadOnlyReactiveProperty<ulong> HealthAmount { get; }
        public ReadOnlyReactiveProperty<ulong> AttackAmount { get; }
        public ReadOnlyReactiveProperty<ulong> DefenseAmount { get; }

        public HeroViewModel(HeroModel model, HeroEquipmentModelProxy heroEquipment)
        {
            _model = model;
            HeroEquipment = heroEquipment;
            HeroNameValue = _model.HeroName;
            LevelAmount = _model.Level;
            ExperienceAmount = _model.Experience;
            HealthAmount = _model.Health;
            AttackAmount = _model.Attack;
            DefenseAmount = _model.Defense;
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}