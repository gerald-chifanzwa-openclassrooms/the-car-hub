using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarHub.Models
{
    public class VehicleMakeViewModel
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
