namespace Ecommerce.Dtos;

public class ProductItemDto
{
    public ProductDto Product { get; set; } = default!;
    public int Quantity { get; set; }
}