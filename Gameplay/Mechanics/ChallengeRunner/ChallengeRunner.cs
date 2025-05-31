using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ink.Runtime;
using Model;
using R3;
using Service;
using ViewModel;

namespace Gameplay
{
    public class ChallengeRunner : IChallengeRunner, IDisposable
    {
        private readonly StoryViewModel _viewModel;
        private readonly StoryModel _storyModel;
        private readonly IDebugService _debugService;
        private readonly CompositeDisposable _compositeDisposable = new();
        private Story _story;
        private int _prevDay = -1;
        public bool IsCanContinue => _story?.canContinue ?? false;
        private Action<string> _eventHandler;
        private Action<int> _choiceHandler;
        private Action<List<Choice>> _choiceSelectionHandler;
        private Action _needEventHandler;
        private Action<IReadOnlyDictionary<string, string>> _tags;

        private ChallengeRunner(string json, StoryViewModel viewModel, StoryModel storyModel,
            IDebugService debugService)
        {
            _storyModel = storyModel;
            _viewModel = viewModel;
            _debugService = debugService;
            _story = CreateStoryFromJson(json);
        }

        public void Configure(string json)
        {
            _story = CreateStoryFromJson(json);
        }

        public void SetEventHandler(Action<string> eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public void SetTagHandler(Action<IReadOnlyDictionary<string, string>> tags)
        {
            _tags = tags;
        }

        public void SetNeedEventHandler(Action needEventHandler)
        {
            _needEventHandler = needEventHandler;
        }

        public void SetChoiceDecisionHandler(Action<List<Choice>> choiceSelectionHandler)
        {
            _choiceSelectionHandler = choiceSelectionHandler;
        }

        public void SetChoiceHandler(Action<int> choiceHandler)
        {
            _choiceHandler = choiceHandler;
        }

        public UniTask Init()
        {
            _viewModel.NextStepCommand
                .Subscribe(x => Move())
                .AddTo(_compositeDisposable);
            _viewModel.UserChoiceCommand
                .Subscribe(MakeChoice)
                .AddTo(_compositeDisposable);

            Move();
            return UniTask.CompletedTask;
        }

        public void GoToEvent(string eventName)
        {
            _story?.ResetState();
            _story?.ChoosePathString(eventName);
            Move();
            _eventHandler?.Invoke(eventName);
        }

        public void MakeChoice(int choiceIndex)
        {
            _story?.ChooseChoiceIndex(choiceIndex);
            Move();
            _choiceHandler?.Invoke(choiceIndex);
        }

        private void Move()
        {
            if (_story == null) return;

            if (IsCanContinue)
            {
                var text = _story.Continue();
                if (string.IsNullOrEmpty(text) || text == "\n")
                {
                    _debugService.LogError($"{_story.state.currentPathString}");
                    text = _story.Continue(); // HACK:
                }

                _debugService.Log(text);
                var model = _storyModel.CurrentDay != _prevDay
                    ? new ChallengeModel(text, _storyModel.CurrentDay)
                    : new ChallengeModel(text);

                UpdateChallengeModel(model);
                _viewModel.UpdateView(model);
                _tags?.Invoke(ParseTags(_story.currentTags));
            }
            else if (_story.currentChoices.Count > 0)
            {
                _choiceSelectionHandler?.Invoke(_story.currentChoices);
            }
            else
            {
                _needEventHandler?.Invoke();
            }

            _prevDay = _storyModel.CurrentDay;
        }

        private Story CreateStoryFromJson(string json)
        {
            var story = new Story(json)
            {
                variablesState =
                {
                    ["player_name"] = "<color=green>Даниил</color>",
                    ["player_health"] = _storyModel.PlayerHealth,
                    ["player_attack"] = _storyModel.PlayerAttack
                }
            };
            story.onError += (msg, type) => { _debugService.LogError($"Ink Error: {msg} (Type: {type})"); };
            return story;
        }

        public void Dispose()
        {
            _story = null;
            _eventHandler = null;
            _choiceHandler = null;
            _choiceSelectionHandler = null;
            _compositeDisposable?.Dispose();
        }

        #region old

        private void UpdateChallengeModel(ChallengeModel challengeModel)
        {
            if (_story.currentChoices.Count <= 0) return;
            if (challengeModel?.Choices == null) return;
            challengeModel.Choices.Clear();
            foreach (var choice in _story.currentChoices)
            {
                challengeModel.Choices.Add(choice.text);
                var choiceTags = _story.TagsForContentAtPath(choice.targetPath.ToString());

                if (choiceTags != null) ProcessChoiceTags(choiceTags, choice);
            }
        }

        private void ProcessChoiceTags(List<string> choiceTags, Choice choice)
        {
            var choiceAttributes = ParseTags(choiceTags);

            if (choiceAttributes.TryGetValue("difficulty", out var difficulty))
                _debugService.Log($"{choice.text}\ndifficulty is {difficulty}\n");

            if (choiceAttributes.TryGetValue("reward", value: out var reward))
                _debugService.Log($"{choice.text}\nreward is {reward}\n");
        }

        private IReadOnlyDictionary<string, string> ParseTags(List<string> tags)
        {
            var attributes = new Dictionary<string, string>();

            foreach (var tag in tags)
            {
                var splitTag = tag.Split(':');
                if (splitTag.Length < 2) continue;
                var key = splitTag[0].Trim();
                var value = splitTag[1].Trim();
                attributes[key] = value;
            }

            return attributes;
        }

        #endregion
    }
}