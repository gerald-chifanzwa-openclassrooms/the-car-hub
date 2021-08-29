using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarHub.Models
{
    public class ChangePasswordViewModel
    {
        [Required, Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        [Required, Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required, Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
