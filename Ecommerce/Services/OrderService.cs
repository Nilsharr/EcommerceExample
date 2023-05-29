using Ecommerce.Dtos;
using Ecommerce.Enums;
using Ecommerce.Interfaces;

namespace Ecommerce.Services;

public class OrderService : IOrderService
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public OrderService(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public OrderDto CompleteOrderDefinition(OrderDto orderDto, string userId, bool isNew)
    {
        if (isNew)
        {
            orderDto.CreatedAt = _dateTimeProvider.Now;
            orderDto.Status = OrderStatus.Pending;
        }

        var netto = CalculateNettoPrice(orderDto.Items);
        var brutto = CalculateBruttoPrice(netto, orderDto.PaymentDetails.Tax);

        orderDto.UserId = userId;
        orderDto.UpdatedAt = _dateTimeProvider.Now;
        orderDto.PaymentDetails.NettoPrice = netto;
        orderDto.PaymentDetails.BruttoPrice = brutto;

        return orderDto;
    }

    private static decimal CalculateNettoPrice(IEnumerable<ProductItemDto> items)
    {
        return items.Sum(x => x.Quantity * x.Product.Price);
    }

    private static decimal CalculateBruttoPrice(decimal nettoPrice, decimal taxRate)
    {
        return nettoPrice + nettoPrice * (taxRate / 100);
    }
}