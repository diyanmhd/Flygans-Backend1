using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Orders;
using Flygans_Backend.Repositories.Products;
using Flygans_Backend.Repositories.Carts;

namespace Flygans_Backend.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICartRepository _cartRepo;

        public OrderService(
            IOrderRepository orderRepo,
            IProductRepository productRepo,
            ICartRepository cartRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _cartRepo = cartRepo;
        }

        private string GenerateOrderNumber()
        {
            return "ORD" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public async Task<ServiceResponse<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var response = new ServiceResponse<OrderResponseDto>();

            // Get user's cart
            var cart = await _cartRepo.GetByUser(userId);
            if (cart == null)
            {
                response.Success = false;
                response.Message = "Your cart is empty. Add items to cart before checkout.";
                return response;
            }

            var cartItems = await _cartRepo.GetItemsByCart(cart.Id);

            // VALIDATION → ORDER MUST MATCH CART EXACTLY
            foreach (var orderItem in dto.Items)
            {
                var matchingCartItem = cartItems.FirstOrDefault(c =>
                    c.ProductId == orderItem.ProductId &&
                    c.Quantity == orderItem.Quantity
                );

                if (matchingCartItem == null)
                {
                    response.Success = false;
                    response.Message = "You can only checkout items already in your cart.";
                    return response;
                }
            }

            // If we reach here → Items match cart
            // Build and save order
            var order = new Order
            {
                UserId = userId,
                DeliveryAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                OrderNumber = GenerateOrderNumber(),
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = await _productRepo.GetProductByIdAsync(item.ProductId);

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                totalAmount += product.Price * item.Quantity;
            }

            order.TotalAmount = totalAmount;

            var savedOrder = await _orderRepo.CreateOrderAsync(order);

            // REMOVE ITEMS FROM CART
            foreach (var orderItem in dto.Items)
            {
                var matchingCartItem = cartItems
                    .First(c => c.ProductId == orderItem.ProductId &&
                                c.Quantity == orderItem.Quantity);

                await _cartRepo.RemoveItem(matchingCartItem);
            }

            await _cartRepo.Save();

            response.Data = MapOrderToDto(savedOrder);
            response.Message = "Order placed successfully.";

            return response;
        }

        public async Task<ServiceResponse<List<OrderResponseDto>>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);

            return new ServiceResponse<List<OrderResponseDto>>
            {
                Data = orders.Select(MapOrderToDto).ToList()
            };
        }

        public async Task<ServiceResponse<OrderResponseDto>> GetOrderByIdAsync(int orderId, int userId)
        {
            var response = new ServiceResponse<OrderResponseDto>();
            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                response.Success = false;
                response.Message = "Order not found";
                return response;
            }

            if (order.UserId != userId)
            {
                response.Success = false;
                response.Message = "Unauthorized access";
                return response;
            }

            response.Data = MapOrderToDto(order);
            return response;
        }

        private OrderResponseDto MapOrderToDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                DeliveryAddress = order.DeliveryAddress,
                PaymentMethod = order.PaymentMethod,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                Items = order.OrderItems.Select(i => new OrderItemResponseDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}
