using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CognitiveBot.BusinessLogic
{
    public class ThesaurusRequest
    {
        private const string Endpoint = "http://thesaurus.altervista.org/thesaurus/v1";

        public async Task<string> GetResponse(string word, string language, string key, string output)
        {
            var uri = new Uri($"{Endpoint}?word={HttpUtility.UrlEncode(word, Encoding.UTF8)}&language={language}&key={key}&output={output}");

            var request = WebRequest.CreateHttp(uri);
            var webResponse = request.GetResponse();
            var streamResponse = webResponse.GetResponseStream();

            if (streamResponse == null) return string.Empty;
            using (var streamRead = new StreamReader(streamResponse))
            {

                return await streamRead.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }


    public class ThesaurusList
    {
        public string category { get; set; }
        public string synonyms { get; set; }
    }

    public class ThesaurusResponse
    {
        public ThesaurusList list { get; set; }
    }

    public class ThesaurusRoot
    {
        public List<ThesaurusResponse> response { get; set; }
    }
}
