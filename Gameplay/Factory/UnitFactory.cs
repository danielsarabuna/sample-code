using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gameplay.Factory;
using Model;
using Model.Actor;
using Service;
using VContainer;
using VContainer.Unity;
using View;
using HeroModel = Model.Actor.HeroModel;

namespace Gameplay.Battle
{
    public class CharacterUnit : Unit
    {
        protected CharacterUnit(string name) : base(name)
        {
        }
    }

    public class CharacterUnit<TModel, TView> : CharacterUnit
        where TModel : ActorModel
        where TView : GameEntityView
    {
        protected readonly TModel Model;
        protected readonly TView EntityView;

        protected CharacterUnit(string name, TModel model, TView entityView) : base(name)
        {
            Model = model;
            EntityView = entityView;
        }
    }

    public class PlayerUnit : CharacterUnit<HeroModel, PlayerEntityView>
    {
        internal PlayerUnit(string name, HeroModel model, PlayerEntityView entityView) : base(name, model, entityView)
        {
            Health = Convert.ToSingle(model.Data[nameof(Attack)]);
            Mana = 0;
            Attack = Convert.ToSingle(model.Data[nameof(Attack)]);
            Defense = Convert.ToSingle(model.Data[nameof(Defense)]);
        }
    }

    public class EnemyUnit : CharacterUnit<EnemyModel, EnemyEntityView>
    {
        internal EnemyUnit(string name, EnemyModel model, EnemyEntityView entityView) : base(name, model, entityView)
        {
            Health = Convert.ToSingle(model.Data[nameof(Health)]);
            Attack = Convert.ToSingle(model.Data[nameof(Attack)]);
            Defense = Convert.ToSingle(model.Data.GetValueOrDefault(nameof(Defense)));
        }
    }

    public class UnitFactory
    {
        private const string PrefabPath = "PrefabPath";

        private readonly IObjectResolver _resolver;
        private readonly IAssetManagement _assetManagement;
        private readonly ActorModelFactory _actorModelFactory;
        private readonly AbilityFactory _abilityFactory;
        private readonly SpawnManager _spawnManager;
        private readonly IUserGameDetails _userGameDetails;

        public UnitFactory(IObjectResolver resolver)
        {
            _resolver = resolver;
            _assetManagement = resolver.Resolve<IAssetManagement>();
            _actorModelFactory = resolver.Resolve<ActorModelFactory>();
            _spawnManager = resolver.Resolve<SpawnManager>();
            _abilityFactory = resolver.Resolve<AbilityFactory>();
            _userGameDetails = resolver.Resolve<IUserGameDetails>();
        }

        public UniTask<PlayerUnit> CreatePlayer(PlayerEntityView view)
        {
            var model = _actorModelFactory.CreateHeroModel(_userGameDetails, "Kaganobu", HeroClass.Mage);
            var unit = new PlayerUnit(model.Name, model, view) { TeamID = 0 };
            unit.Abilities.Add(_abilityFactory.CreateDamage("Slash", 1F, unit.Attack));
            unit.Abilities.Add(_abilityFactory.CreateDamage("Heavy Strike", 3F, 35F));
            unit.Abilities.Add(_abilityFactory.CreateDot("Bleed", 5F, 3F, 5F));
            return UniTask.FromResult(unit);
        }

        public async UniTask<EnemyUnit> CreateEnemy(EnemyModel model)
        {
            var path = model.Data[PrefabPath].ToString();
            var view = await _assetManagement.Instantiate<EnemyEntityView>(path);
            _spawnManager.SpawnEnemy(view);
            _resolver.InjectGameObject(view.gameObject);
            var unit = new EnemyUnit(model.Name, model, view) { TeamID = 1 };
            unit.Abilities.Add(_abilityFactory.CreateDamage("Slash", 1F, unit.Attack));
            unit.Abilities.Add(_abilityFactory.CreateDamage("Fireball", 1F, 25F));
            unit.Abilities.Add(_abilityFactory.CreateHeal("Heal", 4F, 20F));
            unit.Abilities.Add(_abilityFactory.CreateDamageAddition("Burn", 4F,
                _abilityFactory.CreateDot("Burn", 4F, 3F, 7F), 7F));
            return unit;
        }
    }
}