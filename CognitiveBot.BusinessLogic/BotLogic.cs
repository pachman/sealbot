using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Linguistics;
using Microsoft.ProjectOxford.Linguistics.Contract;
using Newtonsoft.Json;

namespace CognitiveBot.BusinessLogic
{
    public class BotLogic
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LinguisticsClient"/> class.
        /// </summary>
        private static readonly LinguisticsClient Client = new LinguisticsClient(SettingsConstants.LinguisticsClientKey);

        public async Task<object> Process(string inputText)
        {
            // List analyzers
            Analyzer[] supportedAnalyzers = null;
            try
            {
                supportedAnalyzers = await ListAnalyzers().ConfigureAwait(false);
#if DEBUG
                var analyzersAsJson = JsonConvert.SerializeObject(supportedAnalyzers, Formatting.Indented, SettingsConstants.JsonSerializerSettings);
                Trace.WriteLine("Supported analyzers: " + analyzersAsJson);
#endif
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Failed to list supported analyzers: {e}");
            }

            // Analyze text with all available analyzers
            if (supportedAnalyzers != null)
            {
                var analyzeTextRequest = new AnalyzeTextRequest()
                {
                    Language = "en",
                    AnalyzerIds = supportedAnalyzers.Select(analyzer => analyzer.Id).ToArray(),
                    Text = inputText
                };

                try
                {
                    var analyzeTextResults = await AnalyzeText(analyzeTextRequest).ConfigureAwait(false);

#if DEBUG
                    var resultsAsJson = JsonConvert.SerializeObject(analyzeTextResults, Formatting.Indented, SettingsConstants.JsonSerializerSettings);
                    Trace.WriteLine("Analyze text results: " + resultsAsJson);
#endif

                    return analyzeTextResults.FirstOrDefault(r =>
                    {
                        Guid guid = Guid.Parse("22a6b758-420f-4745-8a3c-46835a67c0d2");
                        return r.AnalyzerId == guid;
                    })?.Result;
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Failed to list supported analyzers: {e}");
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// List analyzers synchronously.
        /// </summary>
        /// <returns>An array of supported analyzers.</returns>
        private async Task<Analyzer[]> ListAnalyzers()
        {
            try
            {
                return await Client.ListAnalyzersAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to gather list of analyzers", exception as ClientException);
            }
        }

        /// <summary>
        /// Analyze text synchronously.
        /// </summary>
        /// <param name="request">Analyze text request.</param>
        /// <returns>An array of analyze text result.</returns>
        private async Task<AnalyzeTextResult[]> AnalyzeText(AnalyzeTextRequest request)
        {
            try
            {
                return await Client.AnalyzeTextAsync(request).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to analyze text", exception as ClientException);
            }
        }
    }
}