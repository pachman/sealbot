using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CognitiveBot.BusinessLogic
{
    public static class SynonymReplacer
    {
        private static readonly ThesaurusRequest ThesaurusRequest = new ThesaurusRequest();

        private static readonly BotLogic BotLogic = new BotLogic();

        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public static async Task<string> BigBang(string inputText)
        {
            var process = (dynamic)await BotLogic.Process(inputText).ConfigureAwait(false);
            foreach (var variable in process)
            {
                string value = variable.Value;
                var analysisWord = value.BuildTree();
                var wordsByTag = new List<string>();
                analysisWord.DetourTree("NNP", wordsByTag);

                Dictionary<string, string> result = new Dictionary<string, string>();

                foreach (var word in wordsByTag)
                {
                    try
                    {
                        var thesaurusResponse = await ThesaurusRequest.GetResponse(word, "en_US", SettingsConstants.ThesaurusKey, "json").ConfigureAwait(false);

                        var thesaurusRoot = JsonConvert.DeserializeObject<ThesaurusRoot>(thesaurusResponse, SettingsConstants.JsonSerializerSettings);
                        var synonymsMany = thesaurusRoot.response.Select(response => response.list.synonyms).ToList();
                        var synonyms = string.Join("|", synonymsMany)
                            .Split('|')
                            .Where(s => !s.EndsWith(")") && !s.Contains(word.ToLowerInvariant()))
                            .ToList();

                        var index = Random.Next(synonyms.Count - 1);
                        var synonym = synonyms[index];

                        result.Add(word, synonym);
                    }
                    catch
                    {
                        result.Add(word, "Cat"); //todo In any not clear situation would be the cat.
                    }
                }

                foreach (var pair in result)
                {
                    inputText = inputText.Replace(pair.Key, pair.Value);
                }
            }
            return inputText;
        }

        private static void DetourTree(this AnalysisWord rootAnalysisWord, string tag, List<string> result)
        {
            if (rootAnalysisWord.Tag.Equals(tag))
            {
                result.Add(rootAnalysisWord.Value);
            }

            foreach (var analysisWord in rootAnalysisWord.Words)
            {
                DetourTree(analysisWord, tag, result);
            }
        }

        private static AnalysisWord BuildTree(this string input)
        {
            AnalysisWord rootWord = null;

            var sbValue = new StringBuilder();

            var analysisParent = new AnalysisWord();
            var analysisChild = new AnalysisWord();

            var split = false;
            foreach (char c in input)
            {
                if (c == ')')
                {
                    if (sbValue.Length > 0)
                    {
                        analysisChild.Value = sbValue.ToString();
                        analysisParent.Words.Add(analysisChild);

                        analysisChild = new AnalysisWord();
                        sbValue.Clear();
                    }
                    continue;
                }

                if (c == '(')
                {
                    if (sbValue.Length > 0)
                    {
                        analysisChild.Tag = sbValue.ToString();
                        analysisParent.Words.Add(analysisChild);
                        analysisParent = analysisChild;

                        if (rootWord == null)
                        {
                            rootWord = analysisParent;
                        }

                        analysisChild = new AnalysisWord();
                        sbValue.Clear();
                    }
                    continue;
                }

                if (c == ' ')
                {
                    split = true;

                    continue;
                }

                if (split)
                {
                    analysisChild.Tag = sbValue.ToString();
                    sbValue.Clear();
                    split = false;
                }

                sbValue.Append(c);
            }

            return rootWord;
        }
    }
}