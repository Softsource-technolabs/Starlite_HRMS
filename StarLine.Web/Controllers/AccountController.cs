using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NToastNotify;
using StarLine.Core.Models;
using StarLine.Infrastructure.Repositories.Employees;
using System.Text;

namespace StarLine.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IToastNotification _toastNotification;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmployeeRepository employeeRepository, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _employeeRepository = employeeRepository;
            _toastNotification = toastNotification;
        }

        public IActionResult Login(string returnUrl = null!)
        {
            returnUrl ??= Url.Content("~/");

            ViewBag.ReturnURL = returnUrl;
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.emailAddress);

                if (user == null)
                {
                    _toastNotification.AddErrorToastMessage("Email Address Not Found");
                    ModelState.AddModelError(string.Empty, "Email Address Not Found");
                    return View(model);
                }

                if (user != null && user.EmailConfirmed == false)
                {
                    _toastNotification.AddInfoToastMessage("Account Confirmation Required");
                    ModelState.AddModelError(string.Empty, "Looks like you haven't confirm your account. Please check your email account for confirm account.");
                    return View(model);
                }

                var userDetails = await _employeeRepository.GetEmployeeByEmail(model.emailAddress);
                if (userDetails != null && userDetails.Data.IsActive == false)
                {
                    ModelState.AddModelError(string.Empty, "Your account is inactive. Please contact the administrator.");
                    return View(model);
                }

                if (userDetails.Data.LoginFirst == true)
                {
                    var resettoken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    _toastNotification.AddWarningToastMessage("Please change your password on first login.");
                    return RedirectToAction("ResetPassword", "Account", new { token = resettoken });
                }
                var password = await _userManager.CheckPasswordAsync(user, model.password);

                var result = await _signInManager.PasswordSignInAsync(model.emailAddress, model.password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var employee = await _employeeRepository.GetEmployeeByEmail(model.emailAddress);
                    if (employee != null)
                    {
                        _toastNotification.AddSuccessToastMessage("Login Successfully");
                        if (await _userManager.IsInRoleAsync(user, "super admin") || await _userManager.IsInRoleAsync(user, "Super Admin- Starlite Owner"))
                        {
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage("Login Failed");
                    }
                }
                _toastNotification.AddErrorToastMessage("Invalid login attempt.");
                return View(model);
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _toastNotification.AddErrorToastMessage("User not found. Please try again later.");
                return RedirectToAction("Login", "Account");
            }
            if (user.EmailConfirmed)
            {
                _toastNotification.AddWarningToastMessage("Account already verified. Please try to login");
                return RedirectToAction("Login", "Account");
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage("Account Verified Successfully");
            }
            else
            {
                _toastNotification.AddSuccessToastMessage("Something wrong while confirming your Account. Try again later.");
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { code = token }, protocol: Request.Scheme);

                    return Redirect(callbackUrl);
                }
                _toastNotification.AddErrorToastMessage("User Not Found");
            }
            ModelState.AddModelError("EmailAddress", "Email Address Not Found");
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _toastNotification.AddErrorToastMessage("Email Address is required for reset Password");
                return RedirectToAction("Login", "Account");
            }
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var model = new ResetPasswordModel { Code = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.EmailAddress);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                    if (result.Succeeded)
                    {
                        var appUser = await _employeeRepository.GetEmployeeByEmail(model.EmailAddress);
                        var employee = appUser.Data;
                        if (employee.LoginFirst == true)
                        {
                            employee.LoginFirst = false;
                            await _employeeRepository.UpdateEmployee(employee);
                        }
                        _toastNotification.AddSuccessToastMessage("Password Change Successfully");
                        return RedirectToAction("Login", "Account");
                    }
                    _toastNotification.AddErrorToastMessage("Error while reseting password");
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                _toastNotification.AddErrorToastMessage("User Not Found");
                ModelState.AddModelError(string.Empty, "User Not Found");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LogOut(string returnUrl)
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            return RedirectToAction("Login", "Account", new { Area = "" });
        }
    }
}
