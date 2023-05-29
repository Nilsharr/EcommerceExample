namespace Ecommerce.Dtos;

public class PaymentDetailDto
{
    public decimal? NettoPrice { get; set; }
    public decimal? BruttoPrice { get; set; }
    public decimal Tax { get; set; }
    public string PaymentMethod { get; set; } = default!;
    public string Status { get; set; } = default!;
}