using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Carts;
using Flygans_Backend.Repositories.Orders;
using Flygans_Backend.Repositories.Products;
using Flygans_Backend.Repositories.Users;
using Flygans_Backend.Exceptions;

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

        public async Task<ServiceResponse<OrderResponseDto>> CreateOrderAsync(
            int userId, CreateOrderDto dto)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);

            if (user == null || user.IsDeleted)
                throw new UnauthorizedException("This account was deleted by an admin.");

            if (user.IsBlocked)
                throw new UnauthorizedException("Your account is blocked. Contact admin.");

            if (dto.Items == null || !dto.Items.Any())
                throw new BadRequestException("No items provided for checkout.");

            var cart = await _cartRepo.GetByUser(userId);

            if (cart == null)
                throw new BadRequestException("Your cart is empty.");

            decimal totalAmount = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var req in dto.Items)
            {
                var cartItem = await _cartRepo.GetItem(cart.Id, req.ProductId);

                if (cartItem == null)
                    throw new NotFoundException(
                        $"Product with ID {req.ProductId} not found in your cart.");

                if (req.Quantity <= 0 || req.Quantity > cartItem.Quantity)
                    throw new BadRequestException(
                        $"Invalid quantity for product ID {req.ProductId}.");

                var product = await _productRepo.GetByIdAsync(req.ProductId);

                if (product == null)
                    throw new NotFoundException(
                        $"Product with ID {req.ProductId} not found.");

                orderItems.Add(new OrderItem
                {
                    ProductId = req.ProductId,
                    Quantity = req.Quantity,
                    UnitPrice = product.Price
                });

                totalAmount += product.Price * req.Quantity;

                if (req.Quantity == cartItem.Quantity)
                {
                    await _cartRepo.RemoveItem(cartItem);
                }
                else
                {
                    cartItem.Quantity -= req.Quantity;
                }
            }

            // ✅ ONLY LOGIC CHANGE (payment-aware status)
            var status =
                dto.PaymentMethod == "Cod"
                    ? OrderStatus.COD
                    : OrderStatus.PendingPayment;

            var order = new Order
            {
                UserId = userId,
                OrderNumber = Guid.NewGuid().ToString("N")[..12],
                DeliveryAddress = dto.DeliveryAddress,
                PaymentMethod = dto.PaymentMethod,
                TotalAmount = totalAmount,
                Status = status, // ✅ FIXED
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _orderRepo.CreateOrderAsync(order);
            await _cartRepo.Save();

            return new ServiceResponse<OrderResponseDto>
            {
                Success = true,
                Message = "Order placed successfully.",
                Data = new OrderResponseDto(order)
            };
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
            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.UserId != userId)
                throw new UnauthorizedException("Unauthorized.");

            return new ServiceResponse<OrderResponseDto>
            {
                Success = true,
                Data = new OrderResponseDto(order)
            };
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
            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status == OrderStatus.Delivered)
                throw new BadRequestException("Delivered orders cannot be deleted.");

            await _orderRepo.DeleteOrder(orderId);

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Order deleted.",
                Data = true
            };
        }

        public async Task<ServiceResponse<bool>> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var success = await _orderRepo.UpdateOrderStatus(orderId, status);

            if (!success)
                throw new NotFoundException("Order not found.");

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Order status updated.",
                Data = true
            };
        }
    }
}
