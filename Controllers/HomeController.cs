﻿using Microsoft.AspNetCore.Mvc;
using MongoDB_Code.Services;
using MongoDB_Code.Models;
using System.Diagnostics;

namespace MongoDB_Code.Controllers
{
    public class HomeController : Controller
    {
        private readonly MongoDBService _mongoDBService; // Alterado para MongoDBService
        private readonly ILogger<HomeController> _logger;

        // Injeção de dependência do MongoDBService diretamente no construtor
        public HomeController(MongoDBService mongoDBService, ILogger<HomeController> logger)
        {
            _mongoDBService = mongoDBService ?? throw new ArgumentNullException(nameof(mongoDBService));
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering Index method in HomeController.");

            if (_mongoDBService == null)
            {
                throw new InvalidOperationException("MongoDBService is not initialized.");
            }

            var model = new IndexModel(_mongoDBService);

            try
            {
                await model.RetrieveDataAsync(10);
                _logger.LogInformation("Data retrieved successfully in HomeController Index method.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving data in HomeController Index method: {ex.Message}");
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Accessing Privacy view in HomeController.");
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogWarning("Entering Error view in HomeController.");

            var settings = _mongoDBService.GetCurrentSettings();
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            {
                _logger.LogWarning("Connection string is empty. Redirecting to Backoffice Index view from HomeController Error method.");
                return RedirectToAction("Index", "Backoffice");
            }

            _logger.LogInformation("Displaying Error view in HomeController.");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
