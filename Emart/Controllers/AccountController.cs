using EMart.Repositories.Interfaces;
using EMart.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMart.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly PasswordHasher<object> _passwordHasher;

        public AccountController(
            IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;

            _passwordHasher =
                new PasswordHasher<object>();
        }

        #region Login GET

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(new LoginVM());
        }

        #endregion


        #region Login POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginVM model,
            string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(
                    "",
                    "Email and password are required.");

                return View(model);
            }


            var account =
                await _accountRepository.Login(
                    model.Email.Trim());


            if (account == null)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }


            if (!account.IsActive)
            {
                ModelState.AddModelError(
                    "",
                    "Your account is inactive. Please contact support.");

                return View(model);
            }


            if (string.IsNullOrWhiteSpace(
                    account.PasswordHash))
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }


            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    account,
                    account.PasswordHash,
                    model.Password);


            if (passwordResult ==
                PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }


            await SignInCustomer(account);


            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }


            return RedirectToAction(
                "Index",
                "Home");
        }

        #endregion


        #region Register GET

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View(new RegisterVM());
        }

        #endregion


        #region Register POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Email is required.");

                return View(model);
            }


            string email =
                model.Email.Trim().ToLower();


            bool emailExists =
                await _accountRepository.EmailExists(
                    email);


            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }


            object passwordUser = new object();


            string passwordHash =
                _passwordHasher.HashPassword(
                    passwordUser,
                    model.Password!);


            int userId =
                await _accountRepository.RegisterCustomer(
                    model,
                    passwordHash);


            if (userId <= 0)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to create your account. Please try again.");

                return View(model);
            }


            var account =
                await _accountRepository.GetCustomerByUserId(
                    userId);


            if (account == null)
            {
                TempData["Success"] =
                    "Registration successful. Please login.";

                return RedirectToAction(
                    nameof(Login));
            }


            await SignInCustomer(account);


            TempData["Success"] =
                "Welcome to EMart! Your account has been created successfully.";


            return RedirectToAction(
                "Index",
                "Home");
        }

        #endregion


        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CustomerCookie");

            HttpContext.Session.Clear();

            TempData["Success"] = "You have been logged out successfully.";

            return RedirectToAction("Index", "Home");
        }

        #endregion


        #region Sign In Customer

        private async Task SignInCustomer(
    CustomerAccountVM account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(
                    nameof(account));
            }

            if (account.UserId <= 0)
            {
                throw new Exception("Invalid UserId.");
            }

            if (account.CustomerId <= 0)
            {
                throw new Exception(
                    "Invalid CustomerId. Customer record was not found.");
            }

            var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            account.UserId.ToString()),

        new Claim(
            "CustomerId",
            account.CustomerId.ToString()),

        new Claim(
            ClaimTypes.Name,
            account.FullName ?? ""),

        new Claim(
            ClaimTypes.Email,
            account.Email ?? "")
    };

            if (!string.IsNullOrWhiteSpace(account.RoleName))
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        account.RoleName));
            }

            var identity = new ClaimsIdentity(
                claims,
                "CustomerCookie");

            var principal =
                new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                "CustomerCookie",
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,

                    ExpiresUtc =
                        DateTimeOffset.UtcNow.AddHours(8),

                    AllowRefresh = true
                });

            HttpContext.Session.SetInt32(
                "CustomerId",
                account.CustomerId);

            HttpContext.Session.SetInt32(
                "UserId",
                account.UserId);
        }

        #endregion
    }
}