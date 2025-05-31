using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    // Система событий (расширена для глобального использования)
    public class GlobalEventManager
    {
        private static Dictionary<string, List<Action<GameEvent>>> _eventHandlers = new();

        public static void Subscribe(string eventType, Action<GameEvent> handler)
        {
            if (!_eventHandlers.ContainsKey(eventType))
                _eventHandlers[eventType] = new List<Action<GameEvent>>();

            _eventHandlers[eventType].Add(handler);
        }

        public static void Unsubscribe(string eventType, Action<GameEvent> handler)
        {
            if (_eventHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                    _eventHandlers.Remove(eventType);
            }
        }

        public static void Trigger(GameEvent gameEvent)
        {
            if (_eventHandlers.TryGetValue(gameEvent.EventType, out var handlers))
            {
                foreach (var handler in handlers)
                    handler.Invoke(gameEvent);
            }
        }
    }

    public interface IEntity
    {
        string Id { get; }
        string Name { get; }
        Vector3 Position { get; }

        T GetComponent<T>() where T : IComponent;
        bool HasComponent<T>() where T : IComponent;
        void AddComponent(IComponent component);
        void RemoveComponent<T>() where T : IComponent;
        void Update();
    }

    public abstract class GameEntity : IEntity
    {
        private readonly Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

        public string Id { get; protected set; }
        public string Name { get; protected set; }
        public Vector3 Position { get; protected set; }

        public T GetComponent<T>() where T : IComponent
        {
            return (T)_components[typeof(T)];
        }

        public bool HasComponent<T>() where T : IComponent
        {
            return _components.ContainsKey(typeof(T));
        }

        public void AddComponent(IComponent component)
        {
            _components[component.GetType()] = component;
            component.Initialize();
        }

        public void RemoveComponent<T>() where T : IComponent
        {
            _components.Remove(typeof(T));
        }

        public void Update()
        {
            foreach (var component in _components.Values)
                component.Update();
        }
    }

    public class TestActor : GameEntity
    {
        public TestActor(string name)
        {
            Name = name;
        }
    }

    public interface IComponent
    {
        void Initialize();
        void Update();
    }

    // Класс для хранения событий
    public class GameEvent
    {
        public string EventType { get; private set; }
        public IEntity Source { get; private set; }
        public object Payload { get; private set; }

        public GameEvent(string eventType, IEntity source, object payload = null)
        {
            EventType = eventType;
            Source = source;
            Payload = payload;
        }
    }

    // Реализация системы эффектов
    public interface IEffect
    {
        string Name { get; }
        float Duration { get; }
        bool IsExpired { get; }
        void Apply(IEntity target);
        void Remove(IEntity target);
        void Update(IEntity target, float deltaTime);
    }

    // Расширенный класс эффектов для гибкости
    public class Effect : IEffect
    {
        public string Name { get; private set; }
        public float Duration { get; private set; }
        public bool IsExpired => Duration <= 0;

        private Action<IEntity> _onApply;
        private Action<IEntity> _onRemove;
        private Action<IEntity, float> _onTick;

        public Effect(string name, float duration, Action<IEntity> onApply, Action<IEntity> onRemove,
            Action<IEntity, float> onTick = null)
        {
            Name = name;
            Duration = duration;
            _onApply = onApply;
            _onRemove = onRemove;
            _onTick = onTick;
        }

        public void Apply(IEntity target)
        {
            _onApply?.Invoke(target);
        }

        public void Remove(IEntity target)
        {
            _onRemove?.Invoke(target);
        }

        public void Update(IEntity target, float deltaTime)
        {
            _onTick?.Invoke(target, deltaTime);
            Duration -= deltaTime;
        }
    }

    // Компонент для управления эффектами
    public class EffectComponent : IComponent
    {
        private readonly IEntity _entity;
        private readonly List<IEffect> _activeEffects = new List<IEffect>();

        public EffectComponent(IEntity entity)
        {
            _entity = entity;
        }

        public void AddEffect(IEffect effect, IEntity target)
        {
            _activeEffects.Add(effect);
            effect.Apply(target);

            GlobalEventManager.Trigger(new GameEvent("EffectApplied", target, effect));
        }

        public void RemoveEffect(IEffect effect, IEntity target)
        {
            if (_activeEffects.Contains(effect))
            {
                effect.Remove(target);
                _activeEffects.Remove(effect);

                GlobalEventManager.Trigger(new GameEvent("EffectRemoved", target, effect));
            }
        }

        public void Initialize()
        {
        }

        public void Update()
        {
            for (var i = _activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = _activeEffects[i];
                effect.Update(_entity, Time.deltaTime);

                if (effect.IsExpired)
                {
                    RemoveEffect(effect, GetCharacter());
                }
            }
        }

        private IEntity GetCharacter() => null; // Реализуйте получение текущего персонажа
    }

    // Расширенная система характеристик
    public class StatsComponent : IComponent
    {
        private readonly IEntity _entity;
        private readonly Dictionary<StatType, float> _baseStats = new();
        private readonly Dictionary<StatType, List<StatModifier>> _modifiers = new();

        public StatsComponent(IEntity entity)
        {
            _entity = entity;
        }

        public void ModifyStat(StatType statType, StatModifier modifier)
        {
            if (!_modifiers.ContainsKey(statType))
                _modifiers[statType] = new List<StatModifier>();

            _modifiers[statType].Add(modifier);

            GlobalEventManager.Trigger(new GameEvent("StatModified", _entity, $"{statType} modified"));
        }

        public void RemoveModifier(StatType statType, StatModifier modifier)
        {
            if (_modifiers.TryGetValue(statType, out var list))
            {
                list.Remove(modifier);
                if (list.Count == 0)
                    _modifiers.Remove(statType);

                GlobalEventManager.Trigger(new GameEvent("StatModifierRemoved", _entity,
                    $"{statType} modifier removed"));
            }
        }

        public float GetFinalStat(StatType statType)
        {
            var finalValue = _baseStats.ContainsKey(statType) ? _baseStats[statType] : 0f;

            if (_modifiers.TryGetValue(statType, out var modifiers))
            {
                foreach (var modifier in modifiers)
                {
                    finalValue = modifier.Apply(finalValue);
                }
            }

            return finalValue;
        }

        public void Initialize()
        {
        }

        public void Update()
        {
            // foreach (var pair in _modifiers)
            // {
            //     var value = GetFinalStat(pair.Key);
            //     var stat = pair.Value.Aggregate(value, (current, statModifier) => statModifier.Apply(current));
            //     _baseStats[pair.Key] = stat;
            //     GlobalEventManager.Trigger(new GameEvent("StatUpdated", _character, $"{pair.Key}: {stat}"));
            // }
        }
    }

    // Компонент для управления анимациями персонажа
    public class AnimationComponent : IComponent
    {
        private IEntity _entity;

        public AnimationComponent(IEntity entity)
        {
            _entity = entity;
        }

        public void PlayAnimation(string animationName)
        {
            Debug.Log($"Playing animation: {animationName} for character {_entity.Name}");
            GlobalEventManager.Trigger(new GameEvent("AnimationPlayed", _entity, animationName));
        }

        public void Initialize()
        {
            // Дополнительная инициализация, если требуется
        }

        public void Update()
        {
            // Обновление состояния анимации, если необходимо
        }
    }

    // Модернизированный AI компонент
    public class AIComponent : IComponent
    {
        private StateMachine _stateMachine;
        private IEntity _entity;

        public AIComponent(IEntity entity)
        {
            _entity = entity;
        }

        public void Initialize()
        {
            InitializeStateMachine();
        }

        public void Update()
        {
            _stateMachine.Update();
        }

        private void InitializeStateMachine()
        {
            _stateMachine = new StateMachine();

            // Добавляем состояния
            _stateMachine.AddState(AIState.Idle, new IdleState(_entity));
            _stateMachine.AddState(AIState.Combat, new CombatState(_entity));
            _stateMachine.AddState(AIState.Hurt, new HurtState(_entity));

            // Устанавливаем переходы
            _stateMachine.AddTransition(AIState.Idle, AIState.Combat, IsEnemyNearby);
            _stateMachine.AddTransition(AIState.Combat, AIState.Idle, () => !IsEnemyNearby());
            _stateMachine.AddTransition(AIState.Combat, AIState.Hurt,
                () => _entity.GetComponent<StatsComponent>().GetFinalStat(StatType.Health) <= 0);
        }

        private bool IsEnemyNearby() => false; // Реализовать
    }

    // Машина состояний
    public class StateMachine
    {
        private Dictionary<AIState, IAIState> _states = new Dictionary<AIState, IAIState>();
        private Dictionary<AIState, List<Transition>> _transitions = new Dictionary<AIState, List<Transition>>();
        private AIState _currentState;

        public void AddState(AIState state, IAIState stateImplementation)
        {
            _states[state] = stateImplementation;
        }

        public void AddTransition(AIState from, AIState to, Func<bool> condition)
        {
            if (!_transitions.ContainsKey(from))
                _transitions[from] = new List<Transition>();

            _transitions[from].Add(new Transition(to, condition));
        }

        public void Update()
        {
            CheckTransitions();
            if (_states.ContainsKey(_currentState))
                _states[_currentState].Update();
        }

        private void CheckTransitions()
        {
            if (_transitions.ContainsKey(_currentState))
            {
                foreach (var transition in _transitions[_currentState])
                {
                    if (transition.Condition())
                    {
                        ChangeState(transition.ToState);
                        break;
                    }
                }
            }
        }

        private void ChangeState(AIState newState)
        {
            if (_states.ContainsKey(_currentState))
                _states[_currentState].Exit();

            _currentState = newState;

            if (_states.ContainsKey(_currentState))
                _states[_currentState].Enter();
        }
    }

    // Вспомогательные классы
    public class Transition
    {
        public AIState ToState { get; private set; }
        public Func<bool> Condition { get; private set; }

        public Transition(AIState toState, Func<bool> condition)
        {
            ToState = toState;
            Condition = condition;
        }
    }

    public interface IAIState
    {
        void Enter();
        void Update();
        void Exit();
    }

    // Пример расширенного состояния
    public class HurtState : IAIState
    {
        private IEntity _entity;

        public HurtState(IEntity entity)
        {
            _entity = entity;
        }

        public void Enter()
        {
            _entity.GetComponent<AnimationComponent>().PlayAnimation("Hurt");
            GlobalEventManager.Trigger(new GameEvent("CharacterHurt", _entity));
        }

        public void Update()
        {
            // Действия в состоянии ранений
        }

        public void Exit()
        {
            // Завершаем состояние
        }
    }

    // Пример состояния 'Простой'
    public class IdleState : IAIState
    {
        private IEntity _entity;

        public IdleState(IEntity entity)
        {
            _entity = entity;
        }

        public void Enter()
        {
            _entity.GetComponent<AnimationComponent>().PlayAnimation("Idle");
            GlobalEventManager.Trigger(new GameEvent("CharacterIdle", _entity));
        }

        public void Update()
        {
            // Здесь могут быть действия, пока персонаж находится в режиме ожидания
        }

        public void Exit()
        {
            // Завершение состояния ожидания
        }
    }

    // Пример состояния 'Бой'
    public class CombatState : IAIState
    {
        private IEntity _entity;

        public CombatState(IEntity entity)
        {
            _entity = entity;
        }

        public void Enter()
        {
            _entity.GetComponent<AnimationComponent>().PlayAnimation("CombatReady");
            GlobalEventManager.Trigger(new GameEvent("CharacterEnteredCombat", _entity));
        }

        public void Update()
        {
            // Логика для ведения боя
        }

        public void Exit()
        {
            // Логика завершения боя
        }
    }


    // Перечисления для управления
    public enum AIState
    {
        Idle,
        Combat,
        Hurt
    }

    public enum StatType
    {
        Strength,
        Agility,
        Intelligence,
        Vitality,
        Health,
        Speed
    }

    // Класс модификатора характеристик
    public class StatModifier
    {
        private ModifierType _type;
        private float _value;

        public StatModifier(ModifierType type, float value)
        {
            _type = type;
            _value = value;
        }

        public float Apply(float baseValue)
        {
            return _type switch
            {
                ModifierType.Flat => baseValue + _value,
                ModifierType.Percent => baseValue * (1 + _value),
                _ => baseValue
            };
        }
    }

    public enum ModifierType
    {
        Flat, // Увеличение или уменьшение значения на фиксированное число
        Percent // Увеличение или уменьшение значения на процент от базового значения
    }
}