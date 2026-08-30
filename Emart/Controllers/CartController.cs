using EMart.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMart.Controllers
{
    [Authorize(AuthenticationSchemes = "CustomerCookie")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        #region Cart List

        public async Task<IActionResult> Index()
        {
            // TODO: Replace with Logged-in CustomerId
            int customerId = Convert.ToInt32(HttpContext.Session.GetInt32("CustomerId"));

            var cart = await _cartRepository.GetCart(customerId);

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }

            return View(cart);
        }

        #endregion

        #region Add To Cart

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            int customerId = Convert.ToInt32(HttpContext.Session.GetInt32("CustomerId"));

            bool result = await _cartRepository.AddToCart(customerId, productId, quantity);

            if (result)
            {
                TempData["Success"] = "Product added successfully.";
            }
            else
            {
                TempData["Error"] = "Unable to add product.";
            }

            return RedirectToAction("Index");
            //return RedirectToAction("Details", "Products", new { id = productId });
        }

        #endregion

        #region Update Quantity

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            if (quantity <= 0)
                quantity = 1;

            await _cartRepository.UpdateQuantity(cartId, quantity);

            return RedirectToAction("Index");
        }

        #endregion

        #region Remove Item

        public async Task<IActionResult> Remove(int cartId)
        {
            await _cartRepository.Remove(cartId);

            TempData["Success"] = "Item removed from cart.";

            return RedirectToAction("Index");
        }

        #endregion

        #region Clear Cart

        public async Task<IActionResult> ClearCart()
        {
            int customerId = Convert.ToInt32(HttpContext.Session.GetInt32("CustomerId"));

            await _cartRepository.ClearCart(customerId);

            TempData["Success"] = "Cart cleared successfully.";

            return RedirectToAction("Index");
        }

        #endregion

        #region Cart Count (AJAX)

        [HttpGet]
        public async Task<JsonResult> GetCartCount()
        {
            // TODO: Replace with Logged-in CustomerId
            int customerId = Convert.ToInt32(HttpContext.Session.GetInt32("CustomerId"));

            int count = await _cartRepository.GetCartCount(customerId);

            return Json(count);
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> MiniCart()
        {
            int customerId = Convert.ToInt32(HttpContext.Session.GetInt32("CustomerId"));

            var items = await _cartRepository.GetMiniCart(customerId);

            return PartialView("_MiniCart", items);
        }
    }
}