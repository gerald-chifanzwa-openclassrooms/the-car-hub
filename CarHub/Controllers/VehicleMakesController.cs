using System.Threading;
using System.Threading.Tasks;
using CarHub.Models;
using CarHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarHub.Controllers
{
    [Authorize]
    public class VehicleMakesController : Controller
    {
        private readonly IVehicleMakeService _vehicleMakeService;

        public VehicleMakesController(IVehicleMakeService vehicleMakeService)
        {
            _vehicleMakeService = vehicleMakeService;
        }

        public async Task<IActionResult> Index()
        {
            var makes = await _vehicleMakeService.GetVehicleMakes(default);
            return View(makes);
        }

        public IActionResult Add() => View();

        [HttpPost]
        public async Task<IActionResult> Add(VehicleMakeViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _vehicleMakeService.AddMake(model, cancellationToken);
            if (result is not null) return RedirectToAction("Index");

            ModelState.AddModelError(string.Empty, "Failed to add make");
            return View(model);
        }
    }
}
