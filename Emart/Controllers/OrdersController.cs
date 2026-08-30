using EMart.Repositories.Interfaces;
using EMart.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EMart.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAddressRepository _addressRepository;

        public OrderController(
            IOrderRepository orderRepository,
            IAddressRepository addressRepository)
        {
            _orderRepository = orderRepository;
            _addressRepository = addressRepository;
        }


        #region Customer ID

        private int? GetCustomerId()
        {
            return HttpContext.Session.GetInt32(
                "CustomerId");
        }

        #endregion


        #region My Orders

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? customerId =
                GetCustomerId();

            if (!customerId.HasValue ||
                customerId.Value <= 0)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var orders =
                await _orderRepository.GetByCustomerId(
                    customerId.Value);


            return View(orders);
        }

        #endregion


        #region Confirmation

        [HttpGet]
        public async Task<IActionResult> Confirmation(
            int id)
        {
            int? customerId =
                GetCustomerId();

            if (!customerId.HasValue ||
                customerId.Value <= 0)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var order =
                await _orderRepository.GetById(
                    id,
                    customerId.Value);


            if (order == null)
            {
                return NotFound();
            }


            var address =
                await _addressRepository.GetById(
                    order.AddressId,
                    customerId.Value);


            var model =
                new OrderConfirmationVM
                {
                    Order = order,
                    Address = address
                };


            return View(model);
        }

        #endregion


        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            int? customerId =
                GetCustomerId();

            if (!customerId.HasValue ||
                customerId.Value <= 0)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var order =
                await _orderRepository.GetById(
                    id,
                    customerId.Value);


            if (order == null)
            {
                return NotFound();
            }


            var details = await _orderRepository.GetOrderDetails(id, customerId.Value);


            var address =
                await _addressRepository.GetById(
                    order.AddressId,
                    customerId.Value);


            var model =
                new OrderVM
                {
                    Order = order,

                    OrderDetails = details,

                    Address = address
                };


            return View(model);
        }

        #endregion
    }
}