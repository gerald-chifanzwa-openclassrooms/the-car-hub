using System.Threading;
using System.Threading.Tasks;
using CarHub.Models;
using CarHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarHub.Controllers
{
    [Authorize]
    [Route("Inventory")]
    public class ManageInventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public ManageInventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellation)
        {
            var vehicles = await _inventoryService.GetAllVehiclesAsync(cancellation);
            return View(vehicles);
        }

        [HttpGet("Add")]
        public IActionResult AddNewCar() => View();

        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewCar(VehicleInputViewModel viewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var vehicle = await _inventoryService.AddVehicle(viewModel, cancellationToken);
            return RedirectToAction("Details", new { vehicle.Id });
        }

        [HttpGet("{id:int}/Details")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var vehicle = await _inventoryService.GetVehicleAsync(id, cancellationToken);
            return vehicle == null ? NotFound() : View(vehicle);
        }

        [HttpGet("{id:int}/Edit")]
        public async Task<IActionResult> EditCar(int id, CancellationToken cancellationToken)
        {
            var vehicle = await _inventoryService.GetVehicleAsync(id, cancellationToken);
            if (vehicle == null) return NotFound();

            VehicleInputViewModel model = new()
            {
                MakeId = vehicle.MakeId,
                Year = vehicle.Year,
                Model = vehicle.Model,
                PurchaseDate = vehicle.PurchaseDate,
                PurchasePrice = vehicle.PurchasePrice,
                Trim = vehicle.Trim
            };
            return View(model);
        }

        [HttpPost("{id:int}/Edit"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(int id, VehicleInputViewModel viewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View();

            var vehicle = await _inventoryService.EditVehicle(id, viewModel, cancellationToken);
            return RedirectToAction("Index");
        }

        [HttpPost("{id:int}/Remove"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCar(VehicleViewModel viewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View();

            var vehicle = await _inventoryService.RemoveVehicle(viewModel.Id, cancellationToken);
            return RedirectToAction("Index");
        }
    }
}
