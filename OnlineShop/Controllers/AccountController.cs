using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Views.ViewModel;
using System.Security.Claims;
using Microsoft.Win32;
using OnlineShop.Models.Identity;
using System.Xml.Linq;

namespace OnlineShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            //    получить информацию о внешней аутентификации пользователя
            //    переданную в ходе процесса аутентификации
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Ошибка от внешнего провайдера. Пожалуйста, попробуйте снова.";
                return RedirectToAction("Error");
            }

            var emailAddress = info.Principal.FindFirstValue(ClaimTypes.Email);
            var existingUser = await _userManager.FindByEmailAsync(emailAddress);

            if (existingUser != null)
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                return RedirectToAction("Products", "Product");
            }

            //попытка выполнить вход на основе данных, 
            //предоставленных внешним провайдером.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Products", "Product");
            }
            else
            {
                // Если пользователь не найден, создайте нового
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var user = new ApplicationUser { UserName = email, Email = email, Name = name };
                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {

                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, name));

                    //связывает пользователя с внешним провайдером
                    await _userManager.AddLoginAsync(user, info);

                    //вход в систему для только что созданного пользователя
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Products", "Product");
                }
            }

            TempData["ErrorMessage"] = "Ошибка авторизации. Пожалуйста, попробуйте снова.";
            return RedirectToAction("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, user.Name));

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Json(new { success = true });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return PartialView("_RegisterForm", model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_LoginForm", model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    return Json(new { success = true });
                }
            }

            ModelState.AddModelError("", "Неверный логин или пароль");
            return PartialView("_LoginForm", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
                return Json(new { success = true });
            }
            return StatusCode(500, new { errorMessage = "Ошибка на сервере. Пожалуйста, попробуйте снова." });
        }



        [HttpGet]
        public IActionResult Error(string errorMessage = null)
        {
            if (errorMessage != null)
            {
                TempData["ErrorMessage"] = errorMessage;
                return View();
            }
            return View();
        }

        [HttpGet]
        public IActionResult IsAuthenticated()
        {
            return Json(new { isAuthenticated = User.Identity.IsAuthenticated });
        }

    }
}









        

    



