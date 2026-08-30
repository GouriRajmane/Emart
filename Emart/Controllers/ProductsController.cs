using EMart.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EMart.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _productsRepository.GetById(id);

            if (model == null || model.Product == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}