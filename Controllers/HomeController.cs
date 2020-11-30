﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomSearchApp.Models;
using AngleSharp.Io;
using Microsoft.Extensions.Configuration;

namespace CustomSearchApp.Controllers
{
    public class HomeController : Controller
    {
        private SearchEngineCollectionContext _context;

        public HomeController(SearchEngineCollectionContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context);
        }

        public IActionResult SearchResults()
        {
            // create new scrape requester and inherit http headers from browser
            var requester = new DefaultHttpRequester();
            requester.Headers["User-Agent"] = Request.Headers["User-Agent"];
            requester.Headers["Accept-Language"] = Request.Headers["Accept-Language"];

            var query = Request.Form["Search"];

            _context.GetSearchResults(requester, query);

            return View(_context);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
