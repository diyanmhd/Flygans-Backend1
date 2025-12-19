using Microsoft.AspNetCore.Http;

namespace Flygans_Backend.DTOs.Product
{
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int StockQuantity { get; set; }

        // optional new image
        public IFormFile? Image { get; set; }
    }
}
