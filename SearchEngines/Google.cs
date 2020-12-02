using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;

namespace CustomSearchApp.SearchEngines
{
    /// <summary>
    /// Class implements Google search model
    /// </summary>
    public class Google : ISearchEngine
    {
        #region Properties

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsSelected { get; set; } = true;

        public Dictionary<string, long> QueryResult { get; set; } = new Dictionary<string, long>();

        public long WordCount => QueryResult.Values.Sum();

        /// <summary>
        /// Google public API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Google Programmable Search Engine ID
        /// </summary>
        public string CxKey { get; set; }

        #endregion

        #region Constructor and initialization

        public Google()
        {
        }

        #endregion

        #region Data acquisition methods

        /// <summary>
        /// Main query method, gets number of search hits for each word from the query
        /// </summary>
        /// <param name="query">Query string</param>
        public async Task GetNrOfSearchRecords(List<string> words, string userAgent=null)
        {
            QueryResult.Clear();

            if (words.Count == 0)
                return;

            // inititalize Google search service
            var customSearchService = new CustomsearchService(new BaseClientService.Initializer { ApiKey = ApiKey });
            var listRequest = customSearchService.Cse.List();
            listRequest.Cx = CxKey;

            // run search for each word separately
            foreach (var word in words)
            {
                listRequest.Q = word;
                var task = await listRequest.ExecuteAsync();
                var isOk = long.TryParse(task.SearchInformation.TotalResults, out var nrOfHits);

                QueryResult.Add(word, isOk ? nrOfHits : 0);
            }
        }

        #endregion
    }
}