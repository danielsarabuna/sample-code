using System;
using System.Collections.Generic;

namespace Gameplay.Battle
{
    // Основное управление состоянием боя
    public interface IBattleStateManager
    {
        bool IsActive { get; }
        bool IsPaused { get; }
        float CurrentTimeScale { get; }
        void StartBattle();
        void StopBattle();
        void ResetBattle();
        void TogglePause();
        void SetBattleSpeed(BattleSpeed speed);
    }

    public interface IUnitManager
    {
        void AddUnit(Unit unit);
        void RemoveUnit(Unit unit);
        IReadOnlyList<Unit> GetAllUnits();
        Unit GetCurrentUnit();
    }

    public interface IUnitEventHandler
    {
        event Action<Unit> OnUnitTurnStarted;
        event Action<Unit> OnUnitDeath;
        event Action<Unit> OnUnitDamaged;
        event Action<Unit> OnUnitHealed;
    }

    public interface IEffectEventHandler
    {
        event Action<Unit, Effect> OnEffectApplied;
        event Action<Unit, Effect> OnEffectRemoved;
    }

    public interface IBattleEventHandler
    {
        event Action OnBattleStarted;
        event Action OnBattleEnded;
        event Action OnBattlePaused;
        event Action OnBattleReseted;
        event Action<BattleSpeed> OnSpeedChanged;
        event Action<bool> OnPauseStateChanged;
    }

    public interface IBattleSystem : IBattleStateManager, IUnitManager, IUnitEventHandler, IEffectEventHandler,
        IBattleEventHandler
    {
    }
}