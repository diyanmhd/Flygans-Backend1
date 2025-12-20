using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Carts;
using Flygans_Backend.Repositories.Orders;
using Flygans_Backend.Repositories.Products;
using Flygans_Backend.Repositories.Users;

namespace Flygans_Backend.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepository _productRepo;
        private readonly IAdminUserRepository _userRepo;

        public OrderService(
            IOrderRepository orderRepo,
            ICartRepository cartRepo,
            IProductRepository productRepo,
            IAdminUserRepository userRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
        }

        public async Task<ServiceResponse<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var response = new ServiceResponse<OrderResponseDto>();

            // validate user
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                response.Success = false;
                response.Message = "This account was deleted by an admin.";
                return response;
            }

            if (user.IsBlocked)
            {
                response.Success = false;
                response.Message = "Your account is blocked. Contact admin.";
                return response;
            }

            // validate checkout items
            if (dto.Items == null || !dto.Items.Any())
            {
                response.Success = false;
                response.Message = "No items provided for checkout.";
                return response;
            }

            // get cart
            var cart = await _cartRepo.GetByUser(userId);
            if (cart == null)
            {
                response.Success = false;
                response.Message = "Your cart is empty.";
                return response;
            }

            decimal totalAmount = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var req in dto.Items)
            {
                var cartItem = await _cartRepo.GetItem(cart.Id, req.ProductId);
                if (cartItem == null)
                {
                    response.Success = false;
                    response.Message = $"Product with ID {req.ProductId} not found in your cart.";
                    return response;
                }

                if (req.Quantity <= 0 || req.Quantity > cartItem.Quantity)
                {
                    response.Success = false;
                    response.Message = $"Invalid quantity for product ID {req.ProductId}.";
                    return response;
                }

                var product = await _productRepo.GetByIdAsync(req.ProductId);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = $"Product with ID {req.ProductId} not found.";
                    return response;
                }

                // add order item without TotalPrice assignment
                orderItems.Add(new OrderItem
                {
                    ProductId = req.ProductId,
                    Quantity = req.Quantity,
                    UnitPrice = product.Price
                });

                totalAmount += product.Price * req.Quantity;

                // update cart
                if (req.Quantity == cartItem.Quantity)
                {
                    await _cartRepo.RemoveItem(cartItem); // remove fully
                }
                else
                {
                    cartItem.Quantity -= req.Quantity; // reduce partially
                }
            }

            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..12],
                DeliveryAddress = dto.DeliveryAddress,
                PaymentMethod = dto.PaymentMethod,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _orderRepo.CreateOrderAsync(order);
            await _cartRepo.Save();

            response.Success = true;
            response.Message = "Order placed successfully.";
            response.Data = new OrderResponseDto(order);

            return response;
        }

        public async Task<ServiceResponse<List<OrderResponseDto>>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);

            return new ServiceResponse<List<OrderResponseDto>>
            {
                Success = true,
                Data = orders.Select(o => new OrderResponseDto(o)).ToList()
            };
        }

        public async Task<ServiceResponse<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId)
        {
            var response = new ServiceResponse<OrderResponseDto>();

            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found.";
                return response;
            }

            if (order.UserId != userId)
            {
                response.Success = false;
                response.Message = "Unauthorized.";
                return response;
            }

            response.Success = true;
            response.Data = new OrderResponseDto(order);
            return response;
        }

        public async Task<ServiceResponse<List<OrderResponseDto>>> GetAllOrders()
        {
            var orders = await _orderRepo.GetAllOrders();

            return new ServiceResponse<List<OrderResponseDto>>
            {
                Success = true,
                Data = orders.Select(o => new OrderResponseDto(o)).ToList()
            };
        }

        public async Task<ServiceResponse<bool>> DeleteOrder(int orderId)
        {
            var response = new ServiceResponse<bool>();

            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found.";
                return response;
            }

            if (order.Status == OrderStatus.Delivered)
            {
                response.Success = false;
                response.Message = "Delivered orders cannot be deleted.";
                return response;
            }

            await _orderRepo.DeleteOrder(orderId);

            response.Success = true;
            response.Message = "Order deleted.";
            response.Data = true;

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var response = new ServiceResponse<bool>();

            var success = await _orderRepo.UpdateOrderStatus(orderId, status);

            if (!success)
            {
                response.Success = false;
                response.Message = "Order not found.";
                return response;
            }

            response.Success = true;
            response.Message = "Order status updated.";
            response.Data = true;

            return response;
        }
    }
}
