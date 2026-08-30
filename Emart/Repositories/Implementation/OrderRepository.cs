using EMart.Models;
using EMart.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EMart.Repositories.Implementation
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;

        public OrderRepository(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }


        private SqlConnection GetConnection()
        {
            string? connectionString =
                _configuration.GetConnectionString("DBCS");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DBCS' was not found.");
            }

            return new SqlConnection(connectionString);
        }


        #region Place Order

        public async Task<int> PlaceOrder(
            int customerId,
            int addressId,
            string paymentMethod)
        {
            using SqlConnection con =
                GetConnection();

            using SqlCommand cmd =
                new SqlCommand(
                    "SP_Orders",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Flag",
                SqlDbType.VarChar, 30).Value =
                "PLACEORDER";


            cmd.Parameters.Add(
                "@CustomerId",
                SqlDbType.Int).Value =
                customerId;


            cmd.Parameters.Add(
                "@AddressId",
                SqlDbType.Int).Value =
                addressId;


            cmd.Parameters.Add(
                "@PaymentMethod",
                SqlDbType.VarChar, 30).Value =
                paymentMethod;


            await con.OpenAsync();


            object? result =
                await cmd.ExecuteScalarAsync();


            if (result == null ||
                result == DBNull.Value)
            {
                return 0;
            }


            return Convert.ToInt32(result);
        }

        #endregion


        #region Get By Id

        public async Task<Orders?> GetById(
            int orderId,
            int customerId)
        {
            Orders? order = null;


            using SqlConnection con =
                GetConnection();

            using SqlCommand cmd =
                new SqlCommand(
                    "SP_Orders",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Flag",
                SqlDbType.VarChar, 30).Value =
                "GETBYID";


            cmd.Parameters.Add(
                "@OrderId",
                SqlDbType.Int).Value =
                orderId;


            cmd.Parameters.Add(
                "@CustomerId",
                SqlDbType.Int).Value =
                customerId;


            await con.OpenAsync();


            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();


            if (await reader.ReadAsync())
            {
                order = MapOrder(reader);
            }


            return order;
        }

        #endregion


        #region Get By Customer

        public async Task<List<Orders>> GetByCustomerId(
            int customerId)
        {
            List<Orders> orders = new();


            using SqlConnection con =
                GetConnection();

            using SqlCommand cmd =
                new SqlCommand(
                    "SP_Orders",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Flag",
                SqlDbType.VarChar, 30).Value =
                "GETBYCUSTOMER";


            cmd.Parameters.Add(
                "@CustomerId",
                SqlDbType.Int).Value =
                customerId;


            await con.OpenAsync();


            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                orders.Add(
                    MapOrder(reader));
            }


            return orders;
        }

        #endregion


        #region Get Order Details

        public async Task<List<OrderDetails>> GetOrderDetails(
            int orderId,
            int customerId)
        {
            List<OrderDetails> details = new();


            using SqlConnection con =
                GetConnection();

            using SqlCommand cmd =
                new SqlCommand(
                    "SP_Orders",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Flag",
                SqlDbType.VarChar, 30).Value =
                "GETDETAILS";


            cmd.Parameters.Add(
                "@OrderId",
                SqlDbType.Int).Value =
                orderId;


            cmd.Parameters.Add(
                "@CustomerId",
                SqlDbType.Int).Value =
                customerId;


            await con.OpenAsync();


            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                details.Add(
                    new OrderDetails
                    {
                        OrderDetailId =
                            Convert.ToInt32(
                                reader["OrderDetailId"]),

                        OrderId =
                            Convert.ToInt32(
                                reader["OrderId"]),

                        ProductId =
                            Convert.ToInt32(
                                reader["ProductId"]),

                        ProductName =
                            reader["ProductName"]
                                ?.ToString() ?? "",

                        UnitPrice =
                            Convert.ToDecimal(
                                reader["UnitPrice"]),

                        Quantity =
                            Convert.ToInt32(
                                reader["Quantity"]),

                        Total =
                            Convert.ToDecimal(
                                reader["Total"])
                    });
            }


            return details;
        }

        #endregion


        #region Map Order

        private Orders MapOrder(
            SqlDataReader reader)
        {
            return new Orders
            {
                OrderId =
                    Convert.ToInt32(
                        reader["OrderId"]),

                OrderNumber =
                    reader["OrderNumber"]
                        ?.ToString() ?? "",

                CustomerId =
                    Convert.ToInt32(
                        reader["CustomerId"]),

                AddressId =
                    Convert.ToInt32(
                        reader["AddressId"]),

                OrderDate =
                    Convert.ToDateTime(
                        reader["OrderDate"]),

                SubTotal =
                    Convert.ToDecimal(
                        reader["SubTotal"]),

                ShippingAmount =
                    Convert.ToDecimal(
                        reader["ShippingAmount"]),

                DiscountAmount =
                    Convert.ToDecimal(
                        reader["DiscountAmount"]),

                GrandTotal =
                    Convert.ToDecimal(
                        reader["GrandTotal"]),

                OrderStatus =
                    reader["OrderStatus"]
                        ?.ToString() ?? "",

                PaymentMethod =
                    reader["PaymentMethod"]
                        ?.ToString() ?? "",

                CreatedOn =
                    Convert.ToDateTime(
                        reader["CreatedOn"])
            };
        }

        #endregion

        #region Admin - Get All Orders

        public async Task<List<Orders>> GetAllOrders()
        {
            List<Orders> orders = new();

            using SqlConnection con = GetConnection();

            using SqlCommand cmd =
                new SqlCommand(
                    "SP_Orders",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Flag",
                SqlDbType.VarChar,
                30).Value =
                "ADMIN_GETALL";


            await con.OpenAsync();


            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                orders.Add(
                    MapOrder(reader));
            }


            return orders;
        }

        #endregion

        #region Admin - Update Order Status

        public async Task<bool> UpdateOrderStatus(
            int orderId,
            string orderStatus)
        {
            using SqlConnection con =
                GetConnection();

            using SqlCommand cmd =
                new SqlCommand(
                    "SP_Orders",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Flag",
                SqlDbType.VarChar,
                30).Value =
                "ADMIN_UPDATESTATUS";


            cmd.Parameters.Add(
                "@OrderId",
                SqlDbType.Int).Value =
                orderId;


            cmd.Parameters.Add(
                "@OrderStatus",
                SqlDbType.VarChar,
                30).Value =
                orderStatus;


            await con.OpenAsync();


            int rows =
                await cmd.ExecuteNonQueryAsync();


            return rows > 0;
        }

        #endregion

        #region Admin - Get By Id

        public async Task<Orders?> GetByAdminId(
            int orderId)
        {
            Orders? order = null;

            using SqlConnection con =
                GetConnection();

            using SqlCommand cmd =
                new SqlCommand(
                    "SP_Orders",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Flag",
                SqlDbType.VarChar,
                30).Value =
                "ADMIN_GETBYID";


            cmd.Parameters.Add(
                "@OrderId",
                SqlDbType.Int).Value =
                orderId;


            await con.OpenAsync();


            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();


            if (await reader.ReadAsync())
            {
                order =
                    MapOrder(reader);
            }


            return order;
        }

        #endregion

        #region Admin - Get Order Details

        public async Task<List<OrderDetails>>
            GetOrderDetailsByAdmin(
                int orderId)
        {
            List<OrderDetails> details = new();

            using SqlConnection con =
                GetConnection();

            using SqlCommand cmd =
                new SqlCommand(
                    "SP_Orders",
                    con);

            cmd.CommandType =
                CommandType.StoredProcedure;


            cmd.Parameters.Add(
                "@Flag",
                SqlDbType.VarChar,
                30).Value =
                "ADMIN_GETDETAILS";


            cmd.Parameters.Add(
                "@OrderId",
                SqlDbType.Int).Value =
                orderId;


            await con.OpenAsync();


            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                details.Add(
                    new OrderDetails
                    {
                        OrderDetailId =
                            Convert.ToInt32(
                                reader["OrderDetailId"]),

                        OrderId =
                            Convert.ToInt32(
                                reader["OrderId"]),

                        ProductId =
                            Convert.ToInt32(
                                reader["ProductId"]),

                        ProductName =
                            reader["ProductName"]
                                ?.ToString() ?? "",

                        UnitPrice =
                            Convert.ToDecimal(
                                reader["UnitPrice"]),

                        Quantity =
                            Convert.ToInt32(
                                reader["Quantity"]),

                        Total =
                            Convert.ToDecimal(
                                reader["Total"])
                    });
            }


            return details;
        }

        #endregion
    }
}