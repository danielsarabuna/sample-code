using System.Linq;
using Cysharp.Threading.Tasks;
using FSM;
using Gameplay.Battle;
using Model.Actor;
using VContainer;
using View;
using ViewModel;

namespace Gameplay
{
    public class BattleState : IState, IEnter, IExit
    {
        private readonly UnitFactory _unitFactory;
        private readonly StoryViewModel _viewModel;
        private readonly IChallengeRunner _challengeRunner;
        private readonly EnemyModel[] _enemyModels;
        private readonly GameMachine _gameMachine;
        private readonly IBattleSystem _battleSystem;
        private readonly PlayerEntityView _playerEntityView;
        private int _indexEnemy;

        public bool CanSwitch(IState state)
        {
            return true;
        }

        byte IState.ID => StateKey.Battle;

        public BattleState(IObjectResolver resolver)
        {
            _battleSystem = resolver.Resolve<IBattleSystem>();
            _viewModel = resolver.Resolve<StoryViewModel>();
            _playerEntityView = resolver.Resolve<PlayerEntityView>();
            _gameMachine = resolver.Resolve<GameMachine>();
            _challengeRunner = resolver.Resolve<IChallengeRunner>();
            _unitFactory = resolver.Resolve<UnitFactory>();
            _enemyModels = resolver.Resolve<ActorCollection>().Models
                .Where(x => x.GetType() == typeof(EnemyModel))
                .Cast<EnemyModel>()
                .ToArray();
        }

        async void IEnter.Enter()
        {
            InitializeBattleUI();
            await InitializeBattleEntities();
            _battleSystem.OnBattleEnded += BattleSystemOnOnBattleEnded;
        }

        void IExit.Exit()
        {
            _viewModel.ClearChoices();
            _battleSystem.ResetBattle();
            _battleSystem.OnBattleEnded -= BattleSystemOnOnBattleEnded;
        }

        private void InitializeBattleUI()
        {
            _viewModel.IsMoveActive.Value = false;
            _viewModel.IsChoice1Active.Value = true;
            _viewModel.IsChoice2Active.Value = true;

            var choices = _viewModel.Challenges.Last().Choices;
            _viewModel.Choice1Label.Value = choices[0];
            _viewModel.Choice2Label.Value = choices[1];

            _challengeRunner.SetChoiceHandler(HandleChoiceSelection);
        }

        private void BattleSystemOnOnBattleEnded()
        {
            _gameMachine.SetState(StateKey.Dialogue);
        }

        private void HandleChoiceSelection(int index)
        {
            _viewModel.IsChoice1Active.Value = false;
            _viewModel.IsChoice2Active.Value = false;
            if (index > 0) _gameMachine.SetState(StateKey.Dialogue);
            else
            {
                _battleSystem.StartBattle();
            }
            // _challengeRunner.GoToEvent(index >= 1 ? "run_away" : "fight_outcome");
        }

        private async UniTask InitializeBattleEntities()
        {
            var player = await _unitFactory.CreatePlayer(_playerEntityView);
            var enemy = await CreateEnemyUnit();
            
            _battleSystem.AddUnit(player);
            _battleSystem.AddUnit(enemy);

            player.Target = enemy;
            enemy.Target = player;
        }
        
        private async UniTask<EnemyUnit> CreateEnemyUnit()
        {
            var enemyModel = _enemyModels[_indexEnemy++];
            if (_indexEnemy >= _enemyModels.Length) _indexEnemy = 0;
            var enemy = await _unitFactory.CreateEnemy(enemyModel);
            return enemy;
        }
    }
}