using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSearchApp.SearchEngines
{
    /// <summary>
    /// Interface for any search engine model
    /// </summary>
    public interface ISearchEngine
    {
        /// <summary>
        /// Search engine name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Search engine URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// If true, current search engine will be used
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Dictionary containing query result
        /// </summary>
        public Dictionary<string, long> QueryResult { get; set; }

        /// <summary>
        /// Total number of found words
        /// </summary>
        public long WordCount { get; }

        /// <summary>
        /// Main data aquisition method
        /// </summary>
        /// <param name="words">Words to search</param>
        /// <param name="userAgent">User agent</param>
        /// <returns>Async task</returns>
        public Task GetNrOfSearchRecords(List<string> words, string userAgent);
    }
}
