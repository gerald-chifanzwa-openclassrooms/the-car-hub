using System.Threading;
using System.Threading.Tasks;
using CarHub.Models;
using CarHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarHub.Exceptions;
using System;
using CarHub.Data;

namespace CarHub.Controllers
{
    [Authorize]
    [Route("Inventory")]
    public class ManageInventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public ManageInventoryController(IInventoryService inventoryService) => _inventoryService = inventoryService;

        public async Task<IActionResult> Index([FromQuery(Name = "page")] int pageNumber = 1,
                                               [FromQuery(Name = "status")] int? status = null,
                                               CancellationToken cancellation = default)
        {
            LotDisplayStatus? statusFilter = status == null ? null : (LotDisplayStatus)status;
            var request = new GetVehiclesRequest
            {
                IsUserAuthenticated = User.Identity.IsAuthenticated,
                PageNumber = pageNumber,
                PageSize = 20,
                Status = statusFilter,
                Make = null,
            };
            var vehicles = await _inventoryService.GetAllVehiclesAsync(request, cancellation);
            var viewModel = new InventoryListViewModel
            {
                Vehicles = vehicles.Items,
                TotalVehicles = vehicles.TotalCount,
                CurrentPage = vehicles.CurrentPage,
                PageSize = 20,
                StatusFilter = statusFilter
            };
            return View(viewModel);
        }

        [HttpGet("Add")]
        public IActionResult AddNewCar() => View(new VehicleInputViewModel() { PurchaseDate = DateTime.Today });

        [HttpPost("Add"), ValidateAntiForgeryToken]
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
            if (!ModelState.IsValid) return View(viewModel);

            var _ = await _inventoryService.EditVehicle(id, viewModel, cancellationToken);
            return RedirectToAction("Index");
        }

        [HttpPost("{id:int}/Remove"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCar(VehicleViewModel viewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View();

            var _ = await _inventoryService.RemoveVehicle(viewModel.Id, cancellationToken);
            return RedirectToAction("Index");
        }

        [HttpGet("{id:int}/Images")]
        public async Task<IActionResult> UploadImage(int id, CancellationToken cancellationToken)
        {
            var vehicle = await _inventoryService.GetVehicleAsync(id, cancellationToken);
            return vehicle == null ? NotFound() : (IActionResult)View();
        }

        [HttpPost("{id:int}/Images"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int id, VehicleImageUploadViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                await _inventoryService.AddVehicleImage(id, model.File, cancellationToken);
                return RedirectToAction("Details", new { id });
            }
            catch (FileTooLargeException)
            {
                ModelState.AddModelError("", "File is too large");
                return View();
            }
        }

        [HttpGet("{id:int}/AddRepair")]
        public IActionResult AddRepair() => View();

        [HttpPost("{id:int}/AddRepair"), ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRepair(int id, VehicleRepairInputViewModel model, [FromServices] IVehicleRepairsService repairsService, CancellationToken cancellation)
        {
            if (!ModelState.IsValid) return View(model);
            var repair = await repairsService.AddRepair(id, model, cancellation);

            if (repair != null) return RedirectToAction("Details", new { id = repair.VehicleId });

            ModelState.AddModelError("", "Failed to save repair due to an error");
            return View(model);
        }

        [HttpGet("{id:int}/Publish")]
        public async Task<IActionResult> Publish(int id, CancellationToken cancellationToken)
        {
            var vehicleDetails = await _inventoryService.GetVehicleAsync(id, cancellationToken);
            return vehicleDetails switch
            {
                null => NotFound(),
                { Status: Data.LotDisplayStatus.Hidden } => View(new PublishVehicleViewModel(vehicleDetails)),
                _ => RedirectToAction(nameof(Details), new { id })
            };
        }

        [HttpPost("{id:int}/Publish"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id, PublishVehicleViewModel vehicleModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(vehicleModel);
            var vehicle = await _inventoryService.PublishVehicle(id, vehicleModel, cancellationToken);
            return RedirectToAction(nameof(Details), new { id = vehicle.Id });
        }


        [HttpGet("{id:int}/FlagSold")]
        public async Task<IActionResult> FlagSold(int id, CancellationToken cancellationToken)
        {
            var vehicleDetails = await _inventoryService.GetVehicleAsync(id, cancellationToken);
            return vehicleDetails switch
            {
                null => NotFound(),
                { Status: Data.LotDisplayStatus.Show } => View(new VehicleSaleViewModel(vehicleDetails)),
                _ => RedirectToAction(nameof(Details), new { id })
            };
        }

        [HttpPost("{id:int}/FlagSold")]
        public async Task<IActionResult> FlagSold(int id, VehicleSaleViewModel viewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(viewModel);
            var vehicle = await _inventoryService.FlagSoldVehicle(id, viewModel, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

    }
}
