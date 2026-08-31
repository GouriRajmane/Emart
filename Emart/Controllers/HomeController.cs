using Emart.Models;
using EMart.Repositories.Interfaces;
using EMart.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EMart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IBrandsRepository _brandsRepository;
        private readonly IProductsRepository _productsRepository;

        public HomeController(
            ICategoriesRepository categoriesRepository,
            IBrandsRepository brandsRepository,
            IProductsRepository productsRepository)
        {
            _categoriesRepository = categoriesRepository;
            _brandsRepository = brandsRepository;
            _productsRepository = productsRepository;
        }

        //public async Task<IActionResult> Index()
        //{
        //    HomeVM model = new HomeVM();

        //    model.Categories = _categoriesRepository.GetAll();

        //    model.Brands = _brandsRepository.GetAll();

        //    model.LatestProducts = await _productsRepository.GetLatestProducts(8);

        //    model.FeaturedProducts = await _productsRepository.GetFeaturedProducts(8);
        //    model.AllProducts = await _productsRepository.GetAll("", 1, 40);

        //    return View(model);
        //}


        public async Task<IActionResult> Index(int page = 1)
        {
            HomeVM model = new HomeVM();

            model.Categories = _categoriesRepository.GetAll();

            model.Brands = _brandsRepository.GetAll();

            model.LatestProducts =
                await _productsRepository.GetLatestProducts(8);

            model.FeaturedProducts =
                await _productsRepository.GetFeaturedProducts(8);

            model.AllProducts =
                await _productsRepository.GetAll("", page, 2);

            return View(model);
        }










        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
