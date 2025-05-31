using System.Collections.Generic;
using Model;
using ObservableCollections;
using R3;

namespace ViewModel
{
    public class StoryViewModel
    {
        public readonly ReactiveCommand<int> NextStepCommand = new ReactiveCommand<int>();
        public readonly ReactiveCommand<int> UserChoiceCommand = new ReactiveCommand<int>();
        private readonly StoryModel _model;
        private readonly ObservableList<ChallengeModel> _challenges = new ObservableList<ChallengeModel>();
        public ReactiveProperty<bool> IsMoveActive { get; set; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<bool> IsChoice1Active { get; set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsChoice2Active { get; set; } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<string> Choice1Label { get; set; } = new ReactiveProperty<string>(string.Empty);
        public ReactiveProperty<string> Choice2Label { get; set; } = new ReactiveProperty<string>(string.Empty);
        public IReadOnlyObservableList<ChallengeModel> Challenges => _challenges;

        public StoryViewModel(StoryModel model)
        {
            _model = model;
        }

        public void Continue()
        {
            NextStepCommand.Execute(_model.CurrentDay);
        }

        public void ChooseChoice(int choiceIndex)
        {
            UserChoiceCommand.Execute(choiceIndex);
        }

        public void ClearChoices()
        {
            IsMoveActive.Value = true;
            IsChoice1Active.Value = false;
            IsChoice2Active.Value = false;
        }

        public void AddChallenge(ChallengeModel model)
        {
            _challenges.Add(model);
        }

        public void UpdateView(ChallengeModel model)
        {
            _challenges.Add(model);
        }
    }
}