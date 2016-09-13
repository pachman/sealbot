using System.Collections.Generic;

namespace CognitiveBot.BusinessLogic
{
    public class AnalysisWord
    {
        public AnalysisWord()
        {
            Words = new List<AnalysisWord>();
        }

        public string Value { get; set; }

        public string Tag { get; set; }

        public List<AnalysisWord> Words { get; set; }
    }
}