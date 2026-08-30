using EMart.Repositories.Interfaces;
using EMart.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EMart.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IOrderRepository _orderRepository;
        public CheckoutController(ICartRepository cartRepository, IAddressRepository addressRepository, IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _orderRepository = orderRepository;
        }

        #region Checkout

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // =========================================
            // 1. CHECK LOGIN
            // =========================================

            if (User.Identity?.IsAuthenticated != true)
            {
                string returnUrl =
                    Url.Action(
                        "Index",
                        "Checkout") ?? "/Checkout";

                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl = returnUrl
                    });
            }


            // =========================================
            // 2. GET CUSTOMER ID FROM SESSION
            // =========================================

            int? customerId =
                HttpContext.Session.GetInt32("CustomerId");


            if (!customerId.HasValue)
            {
                TempData["Error"] =
                    "Customer session has expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl = "/Checkout"
                    });
            }


            // =========================================
            // 3. GET CART
            // =========================================

            var cart =
                await _cartRepository.GetCart(
                    customerId.Value);


            // =========================================
            // 4. CHECK CART
            // =========================================

            if (cart == null || cart.Count == 0)
            {
                TempData["Error"] =
                    "Your cart is empty.";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }


            // =========================================
            // 5. GET ADDRESSES
            // =========================================

            var addresses =
                await _addressRepository.GetByCustomerId(
                    customerId.Value);


            // =========================================
            // 6. CALCULATE TOTAL
            // =========================================

            decimal subtotal =
                cart.Sum(x => x.Price * x.Quantity);

            decimal shipping = 0;

            decimal discount = 0;

            decimal grandTotal =
                subtotal + shipping - discount;


            // =========================================
            // 7. CREATE CHECKOUT MODEL
            // =========================================

            CheckoutVM model = new CheckoutVM
            {
                CartItems = cart,

                Addresses = addresses,

                SelectedAddressId =
                    addresses.FirstOrDefault()?.AddressId,

                PaymentMethod = "COD",

                Subtotal = subtotal,

                Shipping = shipping,

                Discount = discount,

                GrandTotal = grandTotal
            };


            // =========================================
            // 8. RETURN VIEW
            // =========================================

            return View(model);
        }

        #endregion


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutVM model)
        {
            // =========================================
            // CHECK LOGIN
            // =========================================

            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction(
                    "Login",
                    "Account",
                    new
                    {
                        returnUrl =
                            Url.Action(
                                "Index",
                                "Checkout")
                    });
            }


            // =========================================
            // GET CUSTOMER ID
            // =========================================

            int? customerId = HttpContext.Session.GetInt32("CustomerId");


            if (!customerId.HasValue ||
                customerId.Value <= 0)
            {
                TempData["Error"] =
                    "Customer session expired. Please login again.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }


            // =========================================
            // GET CART
            // =========================================

            var cart =
                await _cartRepository.GetCart(
                    customerId.Value);


            if (cart == null ||
                !cart.Any())
            {
                TempData["Error"] =
                    "Your cart is empty.";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }


            // =========================================
            // VALIDATE ADDRESS
            // =========================================

            if (!model.SelectedAddressId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(model.SelectedAddressId),
                    "Please select a delivery address.");
            }


            // =========================================
            // VALIDATE PAYMENT
            // =========================================

            if (string.IsNullOrWhiteSpace(
                model.PaymentMethod))
            {
                ModelState.AddModelError(
                    nameof(model.PaymentMethod),
                    "Please select a payment method.");
            }


            // =========================================
            // IF INVALID
            // =========================================

            if (!ModelState.IsValid)
            {
                model.CartItems = cart;

                model.Addresses =
                    await _addressRepository.GetByCustomerId(
                        customerId.Value);

                model.Subtotal =
                    cart.Sum(x =>
                        x.Price * x.Quantity);

                model.Shipping = 0;

                model.Discount = 0;

                model.GrandTotal =
                    model.Subtotal;

                return View(
                    "Index",
                    model);
            }


            // =========================================
            // VERIFY ADDRESS
            // =========================================

            var address =
                await _addressRepository.GetById(
                    model.SelectedAddressId.Value,
                    customerId.Value);


            if (address == null)
            {
                ModelState.AddModelError(
                    nameof(model.SelectedAddressId),
                    "Invalid delivery address.");

                model.CartItems = cart;

                model.Addresses =
                    await _addressRepository.GetByCustomerId(
                        customerId.Value);

                model.Subtotal =
                    cart.Sum(x =>
                        x.Price * x.Quantity);

                model.GrandTotal =
                    model.Subtotal;

                return View(
                    "Index",
                    model);
            }


            // =========================================
            // PLACE ORDER
            // =========================================

            try
            {
                int orderId =
                    await _orderRepository.PlaceOrder(
                        customerId.Value,
                        model.SelectedAddressId.Value,
                        model.PaymentMethod);


                if (orderId <= 0)
                {
                    TempData["Error"] =
                        "Unable to place order.";

                    return View(
                        "Index",
                        model);
                }


                // =========================================
                // SUCCESS
                // =========================================

                return RedirectToAction(
                    "Confirmation",
                    "Order",
                    new
                    {
                        id = orderId
                    });
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex);
                ModelState.AddModelError(
                    "",
                    ex.Message);

                model.CartItems = cart;

                model.Addresses =
                    await _addressRepository.GetByCustomerId(
                        customerId.Value);

                model.Subtotal =
                    cart.Sum(x =>
                        x.Price * x.Quantity);

                model.Shipping = 0;

                model.Discount = 0;

                model.GrandTotal =
                    model.Subtotal;

                return View(
                    "Index",
                    model);
            }
        }
    }
}