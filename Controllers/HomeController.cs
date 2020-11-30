using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomSearchApp.Models;
using AngleSharp.Io;
using Config = Microsoft.Extensions.Configuration;

namespace CustomSearchApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly List<SearchEngineModel> _searchEngineModels = new List<SearchEngineModel>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetSearchResults()
        {
            // create new scrape requester and inherit http headers from browser
            var requester = new DefaultHttpRequester();
            requester.Headers["User-Agent"] = Request.Headers["User-Agent"];
            requester.Headers["Accept-Language"] = Request.Headers["Accept-Language"];

            var query = Request.Form["Search"];

            foreach (var model in _searchEngineModels)
            {
                model.GetNrOfSearchRecords(requester, query);
            }

            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
