using EMart.Models;
using EMart.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly IRolesRepository _rolesRepository;

        public RolesController(IRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        // GET: Roles
        public IActionResult Index()
        {
            var roles = _rolesRepository.GetAll();
            return View(roles);
        }

        // GET: Roles/Details/5
        public IActionResult Details(int id)
        {
            var role = _rolesRepository.GetById(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Roles role)
        {
            if (ModelState.IsValid)
            {
                _rolesRepository.Insert(role);
                TempData["Success"] = "Role created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // GET: Roles/Edit/5
        public IActionResult Edit(int id)
        {
            var role = _rolesRepository.GetById(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Roles role)
        {
            if (id != role.RoleId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _rolesRepository.Update(role);
                TempData["Success"] = "Role updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // GET: Roles/Delete/5
        public IActionResult Delete(int id)
        {
            var role = _rolesRepository.GetById(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _rolesRepository.Delete(id);
            TempData["Success"] = "Role deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
