using System.Linq;
using System.Threading.Tasks;
using CarHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarHub.Controllers
{
    [Route("images")]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ImagesController(ApplicationDbContext dbContext) => _dbContext = dbContext;

        [HttpGet("{vehicleId:int}/{imageName}")]
        public async Task<IActionResult> GetImage(int vehicleId, string imageName)
        {
            var image = await _dbContext.VehicleImages
                .FirstOrDefaultAsync(img => img.VehicleId == vehicleId && img.FileName == imageName);
            return image == null ? NotFound() : File(image.ImageData, image.MimeType);
        }
    }
}