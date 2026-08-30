using EMart.Models;
using EMart.ViewModels;

namespace EMart.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> PlaceOrder(
            int customerId,
            int addressId,
            string paymentMethod);

        Task<Orders?> GetById(
            int orderId,
            int customerId);

        Task<List<Orders>> GetByCustomerId(
            int customerId);

        Task<List<OrderDetails>> GetOrderDetails(int orderId,int customerId);

        // =========================================
        // ADMIN ORDER MANAGEMENT
        // =========================================

        Task<Orders?> GetByAdminId(
    int orderId);

        Task<List<OrderDetails>> GetOrderDetailsByAdmin(
            int orderId);

        Task<List<Orders>> GetAllOrders();

        Task<bool> UpdateOrderStatus(
            int orderId,
            string orderStatus);
    }
}