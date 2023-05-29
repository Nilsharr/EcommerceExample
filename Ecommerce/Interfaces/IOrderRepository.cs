using Ecommerce.Dtos;
using Ecommerce.Enums;

namespace Ecommerce.Interfaces;

public interface IOrderRepository : IGenericRepository<OrderDto>
{
    Task<IEnumerable<OrderDto>> GetUserOrders(string userId, OrderStatus? status = null);
    Task<bool> Exists(string id);
    Task UpdateStatus(string id, OrderStatusDto orderStatus);
}