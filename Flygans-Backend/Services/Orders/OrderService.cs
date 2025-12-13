using Flygans_Backend.DTOs.Orders;
using Flygans_Backend.Helpers;
using Flygans_Backend.Models;
using Flygans_Backend.Repositories.Orders;
using Flygans_Backend.Repositories.Products;

namespace Flygans_Backend.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;

        public OrderService(IOrderRepository orderRepo, IProductRepository productRepo)
        {
            _orderRepo = orderRepo;
            _productRepo = productRepo;
        }

        private string GenerateOrderNumber()
        {
            return "ORD" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public async Task<ServiceResponse<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            var response = new ServiceResponse<OrderResponseDto>();

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

                if (product == null)
                {
                    response.Success = false;
                    response.Message = $"Product not found with ID {item.ProductId}";
                    return response;
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.OrderItems.Add(orderItem);

                totalAmount += product.Price * item.Quantity;
            }

            order.TotalAmount = totalAmount;

            var savedOrder = await _orderRepo.CreateOrderAsync(order);

            response.Data = MapOrderToDto(savedOrder);
            response.Message = "Order created successfully";

            return response;
        }

        public async Task<ServiceResponse<List<OrderResponseDto>>> GetOrdersByUserIdAsync(int userId)
        {
            var response = new ServiceResponse<List<OrderResponseDto>>();

            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);

            response.Data = orders.Select(o => MapOrderToDto(o)).ToList();
            return response;
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
