using System.Collections.Generic;
using FSM;
using Ink.Runtime;
using Model;
using UnityEngine;
using VContainer;
using ViewModel;

namespace Gameplay
{
    public static class EventTypes
    {
        private const string SuccessfulCompletion = "event_successful_completion";
        private const string End = "event_end";

        public static bool IsCompletionEvent(string eventName) =>
            eventName is SuccessfulCompletion or End;
    }

    public class DialogueState : IState, IEnter, IExit
    {
        private readonly IEventSystem _eventSystem;
        private readonly IChallengeRunner _challengeRunner;
        private readonly StoryViewModel _viewModel;
        private readonly StoryModel _model;
        private readonly EventList _conditions;
        private readonly IEventChoiceHandler _eventChoiceHandler;
        private readonly GameMachine _gameMachine;
        public byte ID => StateKey.Dialogue;

        public DialogueState(IObjectResolver resolver)
        {
            _eventSystem = resolver.Resolve<IEventSystem>();
            _challengeRunner = resolver.Resolve<IChallengeRunner>();
            _viewModel = resolver.Resolve<StoryViewModel>();
            _model = resolver.Resolve<StoryModel>();
            _eventChoiceHandler = resolver.Resolve<IEventChoiceHandler>();
            _gameMachine = resolver.Resolve<GameMachine>();
            _conditions = new EventList(resolver.Resolve<StoryModel>());
        }

        public bool CanSwitch(IState state) => true;

        public void Enter()
        {
            _challengeRunner.SetEventHandler(OnEventTriggered);
            _challengeRunner.SetChoiceDecisionHandler(OnEventDecision);
            _challengeRunner.SetNeedEventHandler(ExecuteEventLogic);
            _viewModel.IsMoveActive.Value = true;
        }

        public void Exit()
        {
            _challengeRunner.SetEventHandler(null);
            _challengeRunner.SetChoiceDecisionHandler(null);
            _challengeRunner.SetNeedEventHandler(null);
            _viewModel.IsMoveActive.Value = false;
        }

        private void OnEventTriggered(string eventName)
        {
            if (EventTypes.IsCompletionEvent(eventName))
            {
                _viewModel.IsMoveActive.Value = false;
            }

            switch (eventName)
            {
                case "event_enemy_battle":
                case "event_boss_battle":
                    _gameMachine.SetState(StateKey.Battle); break;
            }
        }

        private void OnEventDecision(List<Choice> choices)
        {
            _challengeRunner.MakeChoice(_eventChoiceHandler.MakeChoice(choices));
        }

        private void ExecuteEventLogic()
        {
            var roll = Random.Range(1, 100);

            foreach (var condition in _conditions)
            {
                if (!condition.IsAllowed(roll)) continue;
                if (!_eventSystem.IsEventAllowed(condition.Name, roll)) continue;

                _model.CurrentDay++;
                _eventSystem.ProcessEvent(condition.Name);
                _eventSystem.AddToHistory(condition.Name);
                _challengeRunner.GoToEvent(condition.Name);
                break;
            }
        }
    }
}