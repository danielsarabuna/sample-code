using System.Collections.Generic;
using System.Linq;
using Service;
using VContainer;

namespace Gameplay.Battle
{
    public sealed class CombatSystem
    {
        private readonly IDebugService _debugService;
        private const float BaseTurnDelay = 2F;
        private readonly List<Unit> _units = new List<Unit>();
        private int _currentUnitIndex = 0;
        private float _turnTimer = 0F;
        private float _currentTimeScale = 1F;
        private bool _isPaused = false;
        private bool _isActive = false;

        public IReadOnlyList<Unit> Units => _units;
        public float CurrentTimeScale => _currentTimeScale;
        public bool IsPaused => _isPaused;
        public bool IsActive => _isActive;
        public float TurnTimer => _turnTimer;

        public void AddUnit(Unit unit) => _units.Add(unit);
        public void RemoveUnit(Unit unit) => _units.Remove(unit);
        public Unit GetCurrentUnit() => _units[_currentUnitIndex];
        public void NextTurn() => _currentUnitIndex = (_currentUnitIndex + 1) % _units.Count;
        public void UpdateTurnTimer(float deltaTime) => _turnTimer -= deltaTime;
        public void ResetTurnTimer() => _turnTimer = BaseTurnDelay;


        [Inject]
        public CombatSystem(IDebugService debugService)
        {
            _debugService = debugService;
        }

        public void StartCombat()
        {
            _isActive = true;
        }

        public void StopCombat()
        {
            _isActive = false;
        }

        public void Reset()
        {
            _units.Clear();
            _isPaused = false;
            _isActive = false;
            _turnTimer = 0;
            _currentUnitIndex = 0;
            SetBattleSpeed(BattleSpeed.Normal);
        }

        public void SetBattleSpeed(BattleSpeed speed)
        {
            _currentTimeScale = speed switch
            {
                BattleSpeed.Normal => 1f,
                BattleSpeed.Double => 2f,
                BattleSpeed.Triple => 3f,
                BattleSpeed.Quadruple => 4f,
                _ => 1f
            };
            _debugService.Log($"Battle speed set to {speed} (x{_currentTimeScale})");
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            _debugService.Log(_isPaused ? "Battle Paused" : "Battle Resumed");
        }

        public bool CheckBattleEnd()
        {
            var aliveUnits = _units.Count(u => u.IsAlive);
            if (aliveUnits > 1) return false;
            _isActive = false;
            var winner = _units.FirstOrDefault(u => u.IsAlive);
            _debugService.Log($"\nBattle Ended! {(winner != null ? $"{winner.Name} wins!" : "Draw!")}");
            return true;
        }
    }
}