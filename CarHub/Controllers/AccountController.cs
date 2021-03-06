using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CarHub.Models;
using Microsoft.AspNetCore.Authorization;
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

        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            return Redirect("/");
        }

        public IActionResult ChangePassword() => View();
        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken cancellationToken)
        {
            var userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

            if (user == null) return RedirectToAction(nameof(Login));
            if (!ModelState.IsValid) return View();

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword).ConfigureAwait(false);

            if (result.Succeeded == true) return RedirectToAction("Index", "Home");

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return View(model);
        }
    }
}
