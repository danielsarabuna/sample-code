namespace Common
{
    public interface IBattleStateController
    {
        bool IsActive { get; }
        bool IsPaused { get; }
        float CurrentTimeScale { get; }
        void StartBattle();
        void StopBattle();
        void SetBattleSpeedUp();
        void SetBattleSpeedDown();
    }
}