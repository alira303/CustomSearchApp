using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using Microsoft.AspNetCore.Mvc;

namespace CustomSearchApp.Models
{
    /// <summary>
    /// Base class for any search engine
    /// </summary>
    public class SearchEngineModel
    {
        #region Properties

        /// <summary>
        /// Search engine name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Search engine URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// String for CSS query
        /// </summary>
        public string HtmlQuerySelector { get; set; }

        /// <summary>
        /// If true, this Search engine will be used in data aquisition
        /// </summary>
        [BindProperty]
        public bool IsSelected { get; set; } = true;

        /// <summary>
        /// Dictionary containing result
        /// </summary>
        public Dictionary<string, long> QueryResult { get; set; } = new Dictionary<string, long>();

        /// <summary>
        /// Total number of found words
        /// </summary>
        public long WordCount => QueryResult.Values.Sum();

        #endregion

        #region Constructor and initialization

        public SearchEngineModel()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Data acquisition methods

        public async Task GetNrOfSearchRecords(DefaultHttpRequester requester, string query)
        {
            QueryResult.Clear();

            // input string is empty, or engine not selected, do nothing
            if (string.IsNullOrEmpty(query) || !IsSelected)
                return;

            // remove leading symbols and split the string
            var words = GetWordsFromString(query);
            if (words.Count == 0)
                return;

            // initialize search context
            var config = Configuration.Default.With(requester).WithDefaultLoader();
            var context = BrowsingContext.New(config);

            foreach (var word in words)
            {
                // initialize search form
                var queryDocument = await context.OpenAsync(Url);
                var form = (IHtmlFormElement)queryDocument.QuerySelector("form[action='/search']");

                // send search query, statistics is returned as css query
                var resultDocument = await form.SubmitAsync(new { q = word });
                var stat = resultDocument.QuerySelector(HtmlQuerySelector);

                if (stat == null)
                {
                    QueryResult.Add(word, 0);
                    continue;
                }

                var count = GetFirstNumberSequence(stat.TextContent);
                QueryResult.Add(word, count);
            }
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Splits the incoming string to separate words and removes leading symbols
        /// </summary>
        /// <param name="str">String to split</param>
        /// <returns>List of words</returns>
        private static List<string> GetWordsFromString(string str)
        {
            var result = new List<string>();

            var splitted = str.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var item in splitted)
            {
                result.Add(Regex.Replace(item, "^[^A-Za-z0-9]*", ""));
            }

            return result;
        }

        /// <summary>
        /// Returns the first sequence of digits in a string, converted to long
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>First occurence of digital sequence</returns>
        private static long GetFirstNumberSequence(string str)
        {
            // join blocks of digits together
            var joined = Regex.Replace(str, @"(?<=\d+)\s+(?=\d+)", "");

            // return first block converted to long
            var success = long.TryParse(Regex.Match(joined, @"\d+").Value, out var count);

            return success ? count : 0;
        }

        #endregion
    }
}
