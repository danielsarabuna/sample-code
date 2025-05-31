using System.Collections.Generic;

namespace Model
{
    public class ChallengeModel
    {
        public readonly int? Day;
        public readonly string Text;
        public readonly IList<string> Choices = new List<string>();

        public ChallengeModel(string text)
        {
            Text = text;
        }

        public ChallengeModel(string text, int day)
        {
            Text = text;
            Day = day;
        }
    }
}