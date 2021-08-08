using System.Threading.Tasks;
using CarHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CarHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public ActionResult Login(string returnUrl)
        {
            var viewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);

            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), "User not found");
                model.Password = "";
                return View(model);
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false);

            if (!result)
            {
                ModelState.AddModelError(nameof(model.Password), "Invalid email/password combination");
                model.Password = "";
                return View(model);
            }

            await _signInManager.SignInAsync(user, true).ConfigureAwait(false);
            return Redirect(model.ReturnUrl);
        }
    }
}
