using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(
        AuthenticationSchemes = "AdminCookie",
        Roles = "Admin"
    )]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}