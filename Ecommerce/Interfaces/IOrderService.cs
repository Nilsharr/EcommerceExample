using Ecommerce.Dtos;

namespace Ecommerce.Interfaces;

public interface IOrderService
{
    public OrderDto CompleteOrderDefinition(OrderDto orderDto, string userId, bool isNew);
}