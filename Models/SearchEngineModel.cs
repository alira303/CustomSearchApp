using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Io;

namespace CustomSearchApp.Models
{
    /// <summary>
    /// Base class for any search engine
    /// </summary>
    internal class SearchEngineModel
    {
        #region Properties

        /// <summary>
        /// Search engine name
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Search engine URL
        /// </summary>
        internal string Url { get; set; }

        /// <summary>
        /// String for CSS query
        /// </summary>
        internal string HtmlQuerySelector { get; set; }

        /// <summary>
        /// If true, this Search engine will be used in data aquisition
        /// </summary>
        internal bool IsSelected { get; set; }

        #endregion

        #region Constructor and initialization

        internal SearchEngineModel()
        {
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Data getting methods

        internal async void GetNrOfSearchRecords(DefaultHttpRequester requester, string query)
        {
            var result = new Dictionary<string, long>();

            // input string is empty, do nothing
            if (string.IsNullOrEmpty(query))
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
                var form = (IHtmlFormElement)queryDocument.QuerySelector(HtmlQuerySelector);

                // send search query, statistics is returned as css query
                var resultDocument = await form.SubmitAsync(new { q = word });
                var stat = resultDocument.QuerySelector("div[id='result-stats']");

                if (stat == null)
                {
                    result.Add(word, 0);
                    continue;
                }    

                var count = GetFirstNumberSequence(stat.TextContent);
                result.Add(word, count);
            }
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Splits the incoming string to separate words and removes leading symbols
        /// </summary>
        /// <param name="str">String to split</param>
        /// <returns>List of words</returns>
        private List<string> GetWordsFromString(string str)
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
