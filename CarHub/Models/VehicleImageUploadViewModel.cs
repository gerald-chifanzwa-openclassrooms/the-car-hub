using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;

namespace CarHub.Models
{
    public class VehicleImageUploadViewModel
    {
        [Required]
        [Display(Name = "Select Image File")]
        public IFormFile File { get; set; }
    }
}
