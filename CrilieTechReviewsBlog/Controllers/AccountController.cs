using CrilieTechReviewsBlog.DataManagement.Repositories;
using CrilieTechReviewsBlog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using System;
using System.Threading.Tasks;
using System.Web;

namespace CrilieTechReviewsBlog.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IBlackListedUserRepository _blackListedUser;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailService emailService, IBlackListedUserRepository blackListedUser, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _blackListedUser = blackListedUser;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailAlreadyUsed(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user == null ? Json(true) : Json($"Email {email} is already in use.");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser
                    {
                        UserName = registerVM.Email,
                        Email = registerVM.Email
                    };

                    var result = await _userManager.CreateAsync(user, registerVM.Password);

                    if (result.Succeeded)
                        return await SendAccountConfirmationEmail(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return RedirectToAction("SomethingWentWrong", "Error");
            }
            return View(registerVM);
        }

        private async Task<IActionResult> SendAccountConfirmationEmail(IdentityUser user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(VerifyEmail), "Account", new { userId = user.Id, token }, Request.Scheme, Request.Host.ToString());

                await _emailService.SendAsync(user.Email, "Confirm your CrilieTechReviews Account", $"<a href=\"{link}\">Confirm Account</a>", true);

                return RedirectToAction("EmailVerification", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return RedirectToAction("SomethingWentWrong", "Error");
            }
        }

        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                    return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return RedirectToAction("SomethingWentWrong", "Error");
            }
            return RedirectToAction("SomethingWentWrong", "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendEmailConfirmation(string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);

            if (user != null)
                await SendAccountConfirmationEmail(user);
            else
                return RedirectToAction("SomethingWentWrong", "Error");

            return View("EmailVerification");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(loginVM.Email);

                    if (user != null)
                    {
                        if (_blackListedUser.GetBlacklistedUser(user.Id) != null)
                            return View("AccountIsBlacklisted");

                        var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);

                        if (!user.EmailConfirmed)
                            return View("SendEmailConfirmation", user.Email);

                        if (result.Succeeded)
                            return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return RedirectToAction("SomethingWentWrong", "Error");
            }
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult EmailVerification() => View();

        [HttpGet]
        public IActionResult ResetPasswordNotification() => View();

        [HttpGet]
        public IActionResult SendPasswordResetEmail() => View();

        [HttpPost]
        public async Task<IActionResult> SendPasswordResetEmail(SendPasswordResetTokenViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                    {
                        var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var resetToken = HttpUtility.UrlEncode(rawToken);
                        var link = Url.Action("ResetPassword", "Account",
                                new { email = model.Email, token = resetToken }, Request.Scheme);

                        await _emailService.SendAsync(user.Email, "Reset the password for your CrilieTechReviews Account",
                                                                    $"<a href=\"{link}\">Click here to reset your password</a>", true);

                        return RedirectToAction("ResetPasswordNotification", "Account");
                    }
                    return View("SendPasswordResetEmail");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return RedirectToAction("SomethingWentWrong", "Error");
            }
            return RedirectToAction("SomethingWentWrong", "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
                return RedirectToAction("SomethingWentWrong", "Error");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        var decodedToken = HttpUtility.UrlDecode(model.Token);
                        var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);

                        if (result.Succeeded)
                            return View("ResetPasswordNotification");

                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
            return RedirectToAction("SomethingWentWrong", "Error");
        }

        [HttpGet]
        public IActionResult PasswordResetConfirmation() => View();
    }
}
