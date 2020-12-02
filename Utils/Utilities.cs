using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CustomSearchApp.Utils
{
    /// <summary>
    /// Static utility library
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Splits the incoming string to separate words and removes leading symbols
        /// </summary>
        /// <param name="str">String to split</param>
        /// <returns>List of words</returns>
        internal static List<string> GetWordsFromString(string str)
        {
            var result = new List<string>();

            var splitted = str.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var item in splitted)
            {
                result.Add(Regex.Replace(item, "^[^A-Za-z0-9]*", ""));
            }

            return result;
        }
    }
}
