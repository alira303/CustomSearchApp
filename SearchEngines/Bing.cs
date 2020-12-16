using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomSearchApp.SearchEngines
{
    /// <summary>
    /// Class implements Bing search model
    /// </summary>
    public class Bing : ISearchEngine
    {
        #region Properties

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsSelected { get; set; } = true;

        public Dictionary<string, long> QueryResult { get; set; } = new Dictionary<string, long>();

        public long WordCount => QueryResult.Values.Sum();

        /// <summary>
        /// Bing API key
        /// </summary>
        public string ApiKey { get; set; }

        #endregion


        #region Constructor and initialization

        public Bing()
        {
        }

        #endregion

        #region Data acquisition methods

        /// <summary>
        /// Main query method, gets number of search hits for each word from the query
        /// </summary>
        /// <param name="query">Query string</param>
        public async Task GetNrOfSearchRecords(List<string> words, string userAgent)
        {
            QueryResult.Clear();

            if (words.Count == 0)
                return;

            foreach (var word in words)
            {
                var uriQuery = Url + "?q=" + Uri.EscapeDataString(word) + "&textDecorations=" + Boolean.TrueString;

                // create http client and send request 
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                var response = await client.GetAsync(uriQuery);

                if (!response.IsSuccessStatusCode)
                {
                    QueryResult.Add(word, 0);
                    continue;
                }

                // convert response to json dict
                var content = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);

                // number of hits
                var nrOfHits = ((JToken)searchResponse["webPages"]).SelectToken("totalEstimatedMatches").Value<long>();

                QueryResult.Add(word, nrOfHits);
            }
        }

        #endregion
    }
}
