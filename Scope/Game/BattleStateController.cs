using System;
using Common;
using Gameplay.Battle;
using VContainer;

namespace Root.Game
{
    public class BattleStateController : IBattleStateController // HACK
    {
        private readonly IBattleStateManager _battleStateManager;
        private BattleSpeed _battleSpeed;
        public bool IsActive => _battleStateManager.IsActive;
        public bool IsPaused => _battleStateManager.IsPaused;
        public float CurrentTimeScale => _battleStateManager.CurrentTimeScale;

        [Inject]
        private BattleStateController(IObjectResolver resolver)
        {
            _battleStateManager = resolver.Resolve<IBattleStateManager>();
        }

        public void StartBattle()
        {
            _battleStateManager.StartBattle();
        }

        public void StopBattle()
        {
            _battleStateManager.StopBattle();
        }

        public void SetBattleSpeedUp()
        {
            var battleSpeed = _battleSpeed + 1;
            if (battleSpeed > BattleSpeed.Quadruple) battleSpeed = 0;
            _battleStateManager.SetBattleSpeed(battleSpeed);
            _battleSpeed = battleSpeed;
        }

        public void SetBattleSpeedDown()
        {
            throw new NotImplementedException();
        }
    }
}