using EMart.Models;
using EMart.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMart.Controllers
{
    [Area("Admin")]
    public class BrandsController : Controller
    {
        private readonly IBrandsRepository _brandsRepository;

        public BrandsController(IBrandsRepository brandsRepository)
        {
            _brandsRepository = brandsRepository;
        }

        // GET: Brands
        public IActionResult Index()
        {
            var brands = _brandsRepository.GetAll();
            return View(brands);
        }

        // GET: Brands/Details/5
        public IActionResult Details(int id)
        {
            var brand = _brandsRepository.GetById(id);

            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Brands brand, IFormFile LogoFile)
        {
            if (ModelState.IsValid)
            {
                if (LogoFile != null && LogoFile.Length > 0)
                {
                    string folderPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "brands"
                    );

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Guid.NewGuid().ToString()
                                      + Path.GetExtension(LogoFile.FileName);

                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        LogoFile.CopyTo(stream);
                    }

                    brand.LogoPath = "/uploads/brands/" + fileName;
                }

                _brandsRepository.Insert(brand);

                TempData["Success"] = "Brand created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        // GET: Brands/Edit/5
        public IActionResult Edit(int id)
        {
            var brand = _brandsRepository.GetById(id);

            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Brands brand, IFormFile LogoFile)
        {
            if (id != brand.BrandId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                if (LogoFile != null && LogoFile.Length > 0)
                {
                    string folderPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "brands"
                    );

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = Guid.NewGuid().ToString()
                                      + Path.GetExtension(LogoFile.FileName);

                    string filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        LogoFile.CopyTo(stream);
                    }

                    brand.LogoPath = "/uploads/brands/" + fileName;
                }

                _brandsRepository.Update(brand);

                TempData["Success"] = "Brand updated successfully.";

                return RedirectToAction(nameof(Index)); 
            }

            return View(brand);
        }

        // GET: Brands/Delete/5
        public IActionResult Delete(int id)
        {
            var brand = _brandsRepository.GetById(id);

            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _brandsRepository.Delete(id);
            TempData["Success"] = "Brand deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}