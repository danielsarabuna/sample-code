using System;
using System.Collections.Generic;
using System.Linq;
using Service;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Battle
{
    public class AutoBattleSystem : IBattleSystem, ITickable
    {
        public event Action<Unit> OnUnitTurnStarted;
        public event Action<Unit> OnUnitDeath;
        public event Action<Unit> OnUnitDamaged;
        public event Action<Unit> OnUnitHealed;
        public event Action<Unit, Effect> OnEffectApplied;
        public event Action<Unit, Effect> OnEffectRemoved;
        public event Action OnBattleStarted;
        public event Action OnBattleEnded;
        public event Action OnBattlePaused;
        public event Action OnBattleReseted;
        public event Action<BattleSpeed> OnSpeedChanged;
        public event Action<bool> OnPauseStateChanged;
        private readonly CombatSystem _combatSystem;
        private readonly IDebugService _debugService;
        public bool IsActive => _combatSystem.IsActive;
        public bool IsPaused => _combatSystem.IsPaused;
        public float CurrentTimeScale => _combatSystem.CurrentTimeScale;

        [Inject]
        public AutoBattleSystem(CombatSystem combatSystem, IDebugService debugService)
        {
            _combatSystem = combatSystem;
            _debugService = debugService;
        }

        public void StartBattle()
        {
            _combatSystem.StartCombat();
        }

        public void StopBattle()
        {
            _combatSystem.StopCombat();
            OnBattlePaused?.Invoke();
        }

        public void ResetBattle()
        {
            _combatSystem.Reset();
            OnBattleReseted?.Invoke();
        }

        public void TogglePause()
        {
            _combatSystem.TogglePause();
            OnPauseStateChanged?.Invoke(_combatSystem.IsPaused);
        }

        public void SetBattleSpeed(BattleSpeed speed)
        {
            _combatSystem.SetBattleSpeed(speed);
            OnSpeedChanged?.Invoke(speed);
        }

        public void AddUnit(Unit unit)
        {
            if (unit == null) return;
            _combatSystem.AddUnit(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            if (unit == null) return;
            _combatSystem.RemoveUnit(unit);
            OnUnitDeath?.Invoke(unit);
        }

        public IReadOnlyList<Unit> GetAllUnits()
        {
            return _combatSystem.Units.ToList().AsReadOnly();
        }

        public Unit GetCurrentUnit()
        {
            return _combatSystem.GetCurrentUnit();
        }

        public void Tick()
        {
            if (!_combatSystem.IsActive || _combatSystem.IsPaused) return;

            var scaledDeltaTime = Time.deltaTime * _combatSystem.CurrentTimeScale;

            foreach (var unit in _combatSystem.Units)
                unit.UpdateCooldowns(scaledDeltaTime);

            _combatSystem.UpdateTurnTimer(scaledDeltaTime);

            if (_combatSystem.TurnTimer <= 0)
            {
                ExecuteTurn();
                _combatSystem.ResetTurnTimer();
            }
        }

        private async void ExecuteTurn()
        {
            if (_combatSystem.CheckBattleEnd())
            {
                OnBattleEnded?.Invoke();
                return;
            }

            var currentUnit = _combatSystem.GetCurrentUnit();
            _debugService.Log($"\n{currentUnit.Name}'s turn");
            await currentUnit.PerformTurn();

            _combatSystem.NextTurn();
            LogBattleState();
        }

        private void LogBattleState()
        {
            foreach (var unit in _combatSystem.Units)
            {
                _debugService.Log($"{unit.Name} - HP: {unit.Health}");
            }
        }
    }
}