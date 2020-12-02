using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Io;
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

        public string Name => "Custom search";

        /// <summary>
        /// List of available search engines
        /// </summary>
        [BindProperty]
        public List<SearchEngineModel> Engines { get; private set; } = new List<SearchEngineModel>();

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
                var engine = config.GetSection(item.Key).Get<SearchEngineModel>();
                if (engine == null)
                    continue;

                Engines.Add(engine);
            }
        }

        #endregion

        #region Data acquisition methods

        /// <summary>
        /// Sends queries to all the search engines
        /// </summary>
        /// <param name="requester">Scrape requester</param>
        /// <param name="query">String to search</param>
        public void GetSearchResults(DefaultHttpRequester requester)
        {
            // send queries in parallel and wait for completion
            var listOfTasks = new List<Task>();
            try
            {
                foreach (var engine in Engines)
                {
                    listOfTasks.Add(engine.GetNrOfSearchRecords(requester, Query));
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
