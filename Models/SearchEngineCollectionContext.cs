using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomSearchApp.SearchEngines;
using CustomSearchApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CustomSearchApp.Models
{
    /// <summary>
    /// Class defines collection of search engines
    /// </summary>
    public class SearchEngineCollectionContext
    {
        #region Properties

        public string Name => "Custom search with API";

        /// <summary>
        /// List of available search engines
        /// </summary>
        [BindProperty]
        public List<ISearchEngine> Engines { get; private set; } = new List<ISearchEngine>();

        [BindProperty]
        public string Query { get; set; }

        /// <summary>
        /// Words found by any search engine
        /// </summary>
        [BindProperty]
        public List<string> WordsFound => Engines.Any(engine => engine.IsSelected) ?  Engines.First(t => t.IsSelected).QueryResult.Keys.ToList() : new List<string>();

        #endregion

        #region Constructors and initialization

        public SearchEngineCollectionContext(IConfiguration config)
        {
            // iterate over SearchEngines section of the appsettings.json file
            // and generate available search engine models
            foreach (var item in config.GetSection("SearchEngines").AsEnumerable().
                Where(item => item.Key != "SearchEngines" && item.Value == null))
            {
                var engineName = item.Key.Split(":", StringSplitOptions.RemoveEmptyEntries)[1];
                switch (engineName)
                {
                    case "Google":
                        var google = config.GetSection(item.Key).Get<SearchEngines.Google>();
                        if (google != null)
                            Engines.Add(google);
                        break;
                    case "Bing":
                        var bing = config.GetSection(item.Key).Get<SearchEngines.Bing>();
                        if (bing != null)
                            Engines.Add(bing);
                        break;
                    default:
                        continue;
                }

            }
        }

        #endregion

        #region Data acquisition methods

        /// <summary>
        /// Sends queries to all the search engines
        /// </summary>
        /// <param name="userAgent">User agent string</param>
        public void GetSearchResults(string userAgent)
        {
            // input string is empty, or engine not selected, do nothing
            if (string.IsNullOrEmpty(Query))
                return;

            // remove leading symbols and split the string
            var words = Utilities.GetWordsFromString(Query);

            try
            {
                // send queries in parallel and wait for completion
                var listOfTasks = new List<Task>();
                foreach (var engine in Engines)
                {
                    listOfTasks.Add(engine.GetNrOfSearchRecords(words, userAgent));
                }

                Task.WaitAll(listOfTasks.ToArray());
            }
            catch (Exception)
            {
                return;
            }  
        }

        #endregion
    }
}
