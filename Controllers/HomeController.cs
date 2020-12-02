using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CustomSearchApp.Models;

namespace CustomSearchApp.Controllers
{
    /// <summary>
    /// Controller for Index.cshtml
    /// </summary>
    public class HomeController : Controller
    {
        #region Fields

        /// <summary>
        /// Search engine collection
        /// </summary>
        private SearchEngineCollectionContext _context;

        #endregion

        #region Constructor and initialization

        public HomeController(SearchEngineCollectionContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context);
        }

        #endregion

        #region Custom actions

        /// <summary>
        /// Action returns search ersults
        /// </summary>
        /// <returns>Updated partial view</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SearchResults()
        {
            // search query
            _context.Query = Request.Form["Search"];

            // selected engines
            var selectedEngines = Request.Form["Engines"].ToList();

            // send query to selected engines
            foreach (var engine in _context.Engines)
            {
                engine.IsSelected = selectedEngines.Contains(_context.Engines.IndexOf(engine).ToString());
            }
            _context.GetSearchResults(Request.Headers["User-Agent"]);

            // render the view
            return View(_context);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}
