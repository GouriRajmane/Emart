using EMart.Repositories.Interfaces;
using EMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Authorize(AuthenticationSchemes = "AdminCookie", Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAddressRepository _addressRepository;

        public OrderController(
            IOrderRepository orderRepository,
            IAddressRepository addressRepository)
        {
            _orderRepository =
                orderRepository;

            _addressRepository =
                addressRepository;
        }


        #region Index

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders =
                await _orderRepository.GetAllOrders();

            return View(orders);
        }

        #endregion


        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }


            // -----------------------------------------
            // ADMIN CAN ACCESS ANY ORDER
            // -----------------------------------------

            var order =
                await GetAdminOrder(id);


            if (order == null)
            {
                return NotFound();
            }


            // -----------------------------------------
            // ORDER DETAILS
            // -----------------------------------------

            var details =
                await GetAdminOrderDetails(id);


            // -----------------------------------------
            // DELIVERY ADDRESS
            // -----------------------------------------

            var address =
                await _addressRepository.GetById(
                    order.AddressId,
                    order.CustomerId);


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


        #region Update Status

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int orderId,
            string orderStatus)
        {
            if (orderId <= 0)
            {
                return BadRequest();
            }


            if (string.IsNullOrWhiteSpace(orderStatus))
            {
                TempData["Error"] =
                    "Please select an order status.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = orderId });
            }


            string[] allowedStatuses =
            {
                "Pending",
                "Confirmed",
                "Processing",
                "Shipped",
                "Delivered",
                "Cancelled"
            };


            if (!allowedStatuses.Contains(
                    orderStatus,
                    StringComparer.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    "Invalid order status.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = orderId });
            }


            bool result =
                await _orderRepository
                    .UpdateOrderStatus(
                        orderId,
                        orderStatus);


            if (result)
            {
                TempData["Success"] =
                    "Order status updated successfully.";
            }
            else
            {
                TempData["Error"] =
                    "Unable to update order status.";
            }


            return RedirectToAction(
                nameof(Details),
                new { id = orderId });
        }

        #endregion


        #region Admin Order Helper

        private async Task<EMart.Models.Orders?>
            GetAdminOrder(int orderId)
        {
            /*
             * Existing GetById() requires CustomerId.
             *
             * Admin must not use a customer's
             * CustomerId to access an arbitrary order.
             *
             * Therefore this will be backed by
             * an admin-specific repository method.
             */

            return await _orderRepository
                .GetByAdminId(orderId);
        }

        #endregion


        #region Admin Order Details Helper

        private async Task<List<EMart.Models.OrderDetails>>
            GetAdminOrderDetails(int orderId)
        {
            return await _orderRepository
                .GetOrderDetailsByAdmin(orderId);
        }

        #endregion
    }
}
