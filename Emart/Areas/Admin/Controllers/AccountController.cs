using EMart.Repositories.Interfaces;
using EMart.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly IUsersRepository _usersRepository;

        public AccountController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _usersRepository.Login(model.Email!, model.Password!);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            //if (user.RoleName != "Admin")
            //{
            //    ModelState.AddModelError("", "You are not authorized to access the Admin Panel.");
            //    return View(model);
            //}

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.FullName!
                ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email!
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.RoleName!
                )
            };


            // ================================
            // CREATE IDENTITY
            // ================================

            var identity = new ClaimsIdentity(claims, "AdminCookie");


            // ================================
            // CREATE PRINCIPAL
            // ================================

            var principal = new ClaimsPrincipal(identity);


            // ================================
            // LOGIN
            // ================================

            await HttpContext.SignInAsync("AdminCookie", principal);


            // ================================
            // REDIRECT
            // ================================

            return RedirectToAction("Index", "Dashboard",
                new
                {
                    area = "Admin"
                }
            );
        }


        // ================================
        // LOGOUT
        // ================================

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
    "AdminCookie");


            return RedirectToAction("Login", "Account",
                new
                {
                    area = "Admin"
                }
            );
        }
    }
}