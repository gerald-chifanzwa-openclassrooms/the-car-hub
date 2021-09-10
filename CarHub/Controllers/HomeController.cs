using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CarHub.Models;
using CarHub.Services;
using System.Threading;

namespace CarHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IInventoryService inventoryService, ILogger<HomeController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index([FromQuery(Name = "page")] int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var vehicles = await _inventoryService.GetAllVehiclesAsync(pageNumber, 20, User.Identity.IsAuthenticated, cancellationToken);
            ViewData["TotalItems"] = vehicles.TotalCount;
            ViewData["CurrentPage"] = vehicles.CurrentPage;
            return View(vehicles.Items);
        }

        [HttpGet("{id:int}/Details")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var vehicle = await _inventoryService.GetVehicleAsync(id, cancellationToken);
            return View(vehicle);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
