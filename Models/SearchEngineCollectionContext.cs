using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Io;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CustomSearchApp.Models
{
    public class SearchEngineCollectionContext
    {
        #region Properties

        public string Name => "Custom search";

        /// <summary>
        /// List of available search engines
        /// </summary>
        [BindProperty]
        public List<SearchEngineModel> Engines { get; private set; } = new List<SearchEngineModel>();

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

        #region Data aquisition methods

        /// <summary>
        /// Sends queries to all the search engines
        /// </summary>
        /// <param name="requester">Scrape requester</param>
        /// <param name="query">String to search</param>
        public void GetSearchResults(DefaultHttpRequester requester, string query)
        {
            // send queries in parallel and wait for completion
            var listOfTasks = new List<Task>();
            foreach (var engine in Engines)
            {
                listOfTasks.Add(engine.GetNrOfSearchRecords(requester, query));
            }

            Task.WaitAll(listOfTasks.ToArray());
        }

        #endregion
    }
}
