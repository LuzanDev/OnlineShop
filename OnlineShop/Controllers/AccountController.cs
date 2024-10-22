using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Identity;
using OnlineShop.Views.ViewModel;
using OnlineShop.Views.ViewModel.Password;
using SendGrid;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // Представление для ввода почты на которую прийдет ссылка для сброса пароля
        [HttpGet] 
        public IActionResult ForgotPassword()
        {
            return PartialView();
        }

        // Обработка вводимых даннных (почты) и отправка сообщения с ссылкой для сброса пароля
        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) 
                {
                    return PartialView("ForgotPasswordConfirmation");
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Сброс пароля", $"Вы можете сбросить пароль, нажав на эту ссылку: <a href='{WebUtility.HtmlEncode(callbackUrl)}'>сбросить пароль</a>");
                return PartialView("ForgotPasswordConfirmation");
            }

            return PartialView("ForgotPassword", model);
        }

        // Представление для ввода нового пароля (открывается через почту пользователя)
        [HttpGet]
        public IActionResult ResetPassword(string userId, string code = null)
        {
            if (code == null || userId == null)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при попытке сброса пароля. Пожалуйста, попробуйте снова.";
                return RedirectToAction("Error");
            }
            
            return PartialView(new ResetPasswordViewModel { Code = code, UserId = userId });
        }

        // Обработка вводимых даннных (нового пароля), валидация и изменение пароля пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return PartialView("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return PartialView("ResetPasswordConfirmation");
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Error", new { errorMessage = "Невалидный токен подтверждения" });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("Error", new { errorMessage = "Пользователь не найден" });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return PartialView("EmailConfirmed");
            }

            return RedirectToAction("Error", new { errorMessage = "Не удалось подтвердить электронную почту" });
        }


        // Инициация процесса аутентификации через Google
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        // Обработка результата аутентификации (Google)
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
                // Если пользователь существует, проверяем подтверждена ли почта
                if (!existingUser.EmailConfirmed)
                {
                    TempData["ErrorMessage"] = "Ваша электронная почта не подтверждена. Мы отправили вам новое письмо для подтверждения.";
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token, email = existingUser.Email }, Request.Scheme);
                    
                    await _emailSender.SendEmailAsync(existingUser.Email, "Подтверждение электронной почты", $"Пожалуйста, подтвердите свою почту, перейдя по ссылке: {confirmationLink}");

                    return RedirectToAction("Error");
                }

                await _signInManager.SignInAsync(existingUser, isPersistent: false);
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
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
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
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token, email = user.Email }, Request.Scheme);

                    // Отправляем email через SendGrid
                    await _emailSender.SendEmailAsync(user.Email, "Подтверждение электронной почты", $"Подтвердите вашу почту, пройдя по ссылке: <a href='{confirmationLink}'>подтвердить</a>");

                    return PartialView("EmailConfirmationSent");
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
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Вы не подтвердили свой адрес электронной почты.");
                    return PartialView("_LoginForm", model);
                }
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















