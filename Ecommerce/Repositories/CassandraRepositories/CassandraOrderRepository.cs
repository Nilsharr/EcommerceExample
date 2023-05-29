using AutoMapper;
using Cassandra;
using Cassandra.Data.Linq;
using Ecommerce.Dtos;
using Ecommerce.Entities.Cassandra;
using Ecommerce.Entities.Cassandra.Types;
using Ecommerce.Enums;
using Ecommerce.Interfaces;
using Ecommerce.Utils;
using ISession = Cassandra.ISession;

namespace Ecommerce.Repositories.CassandraRepositories;

public class CassandraOrderRepository : IOrderRepository
{
    private readonly ISession _session;
    private readonly IMapper _mapper;
    private readonly Table<OrdersById> _ordersById;
    private readonly Table<OrdersByUser> _ordersByUser;

    public CassandraOrderRepository(ISession session, IMapper mapper)
    {
        _session = session;
        _mapper = mapper;
        _ordersById = new Table<OrdersById>(_session);
        _ordersByUser = new Table<OrdersByUser>(_session);
    }

    public async Task<IEnumerable<OrderDto>> GetAll()
    {
        var ordersDto = new List<OrderDto>();

        var ordersById = (await _ordersById.ExecuteAsync()).ToList();
        foreach (var orders in ordersById.GroupBy(x => x.OrderId))
        {
            var userId = orders.FirstOrDefault()!.UserId;
            var orderByUser = await _ordersByUser.FirstOrDefault(x => x.UserId == userId && x.OrderId == orders.Key)
                .ExecuteAsync();
            var productItems = GetItemsInOrder(orders).ToList();
            var details = GetDetailsInOrder(orders);
            ordersDto.Add(new OrderDto
            {
                Id = orderByUser.OrderId.ToString(), UserId = orderByUser.UserId.ToString(),
                Status = orderByUser.Status.GetEnum<OrderStatus>(), CreatedAt = orderByUser.CreatedAt,
                UpdatedAt = orderByUser.UpdatedAt, Items = productItems, PaymentDetails = details.paymentDetails,
                ShippingAddress = details.shippingAddress, ShippingMethod = details.shippingMethod
            });
        }

        return ordersDto;
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrders(string userId, OrderStatus? status = null)
    {
        List<OrdersByUser> ordersByUser;
        if (status is not null)
        {
            ordersByUser =
                (await _ordersByUser.Where(x => x.UserId == Guid.Parse(userId) && x.Status == status.Value.GetName())
                    .ExecuteAsync()).ToList();
        }
        else
        {
            ordersByUser = (await _ordersByUser.Where(x => x.UserId == Guid.Parse(userId)).ExecuteAsync()).ToList();
        }

        var ordersDto = new List<OrderDto>();
        foreach (var orderByUser in ordersByUser)
        {
            var ordersById =
                (await _ordersById.Where(x => x.OrderId == orderByUser.OrderId).ExecuteAsync()).ToList();

            var productItems = GetItemsInOrder(ordersById).ToList();
            var details = GetDetailsInOrder(ordersById);

            ordersDto.Add(new OrderDto
            {
                Id = orderByUser.OrderId.ToString(), UserId = userId,
                Status = orderByUser.Status.GetEnum<OrderStatus>(), CreatedAt = orderByUser.CreatedAt,
                UpdatedAt = orderByUser.UpdatedAt, Items = productItems, PaymentDetails = details.paymentDetails,
                ShippingAddress = details.shippingAddress, ShippingMethod = details.shippingMethod
            });
        }

        return ordersDto;
    }

    public async Task<OrderDto?> GetById(string id)
    {
        var ordersById = (await _ordersById.Where(x => x.OrderId == Guid.Parse(id)).ExecuteAsync()).ToList();
        if (ordersById.Count == 0)
        {
            return null;
        }

        var userId = ordersById.Select(x => x.UserId).FirstOrDefault();
        var orderByUser = await _ordersByUser.FirstOrDefault(x => x.UserId == userId && x.OrderId == Guid.Parse(id))
            .ExecuteAsync();

        var productItems = GetItemsInOrder(ordersById).ToList();
        var details = GetDetailsInOrder(ordersById);

        return new OrderDto
        {
            Id = orderByUser.OrderId.ToString(), UserId = userId.ToString(),
            Status = orderByUser.Status.GetEnum<OrderStatus>(), CreatedAt = orderByUser.CreatedAt,
            UpdatedAt = orderByUser.UpdatedAt, Items = productItems, PaymentDetails = details.paymentDetails,
            ShippingAddress = details.shippingAddress, ShippingMethod = details.shippingMethod
        };
    }

    public async Task<bool> Exists(string id)
    {
        // ReSharper disable once ReplaceWithSingleCallToCount
        return await _ordersById.Where(x => x.OrderId == Guid.Parse(id)).Count().ExecuteAsync() > 0;
    }

    public async Task<OrderDto> Add(OrderDto orderDto)
    {
        orderDto.Id = Guid.NewGuid().ToString();
        var paymentDetails = _mapper.Map<PaymentDetail>(orderDto.PaymentDetails);
        var shippingAddress = _mapper.Map<Address>(orderDto.ShippingAddress);
        var shippingMethod = _mapper.Map<ShippingMethod>(orderDto.ShippingMethod);

        var batch = new BatchStatement()
            .Add(_ordersByUser.Insert(new OrdersByUser
            {
                UserId = Guid.Parse(orderDto.UserId!), OrderId = Guid.Parse(orderDto.Id),
                Status = orderDto.Status!.Value.GetName(),
                CreatedAt = orderDto.CreatedAt!.Value, UpdatedAt = orderDto.UpdatedAt!.Value
            }));
        foreach (var item in orderDto.Items)
        {
            batch.Add(_ordersById.Insert(new OrdersById
            {
                OrderId = Guid.Parse(orderDto.Id), UserId = Guid.Parse(orderDto.UserId!),
                Status = orderDto.Status!.Value.GetName(), ProductId = Guid.Parse(item.Product.Id!),
                ProductName = item.Product.Name, ProductDescription = item.Product.Description,
                ProductPrice = item.Product.Price, ProductQuantity = item.Quantity,
                PaymentDetails = paymentDetails, ShippingAddress = shippingAddress, ShippingMethod = shippingMethod
            }));
        }

        await _session.ExecuteAsync(batch);

        return orderDto;
    }

    public async Task<OrderDto> Update(string id, OrderDto orderDto)
    {
        var orderId = Guid.Parse(id);
        var userId = await _ordersById.Where(x => x.OrderId == Guid.Parse(id)).Select(x => x.UserId).FirstOrDefault()
            .ExecuteAsync();
        var userOrders = await _ordersByUser.FirstOrDefault(x => x.UserId == userId && x.OrderId == orderId)
            .ExecuteAsync();

        var paymentDetails = _mapper.Map<PaymentDetail>(orderDto.PaymentDetails);
        var shippingAddress = _mapper.Map<Address>(orderDto.ShippingAddress);
        var shippingMethod = _mapper.Map<ShippingMethod>(orderDto.ShippingMethod);

        var batch = new BatchStatement().Add(_ordersByUser
                .Where(x => x.UserId == userId && x.OrderId == orderId && x.CreatedAt == userOrders.CreatedAt &&
                            x.Status == userOrders.Status)
                .Select(x => new OrdersByUser {UpdatedAt = orderDto.UpdatedAt!.Value}).Update())
            .Add(_ordersById.Where(x => x.OrderId == orderId).Select(x => new OrdersById
            {
                PaymentDetails = paymentDetails, ShippingAddress = shippingAddress, ShippingMethod = shippingMethod
            }).Update());

        await _session.ExecuteAsync(batch);

        return orderDto;
    }

    public async Task UpdateStatus(string id, OrderStatusDto orderStatusDto)
    {
        var orderId = Guid.Parse(id);
        var userId = await _ordersById.Where(x => x.OrderId == orderId).Select(x => x.UserId).FirstOrDefault()
            .ExecuteAsync();
        var orderCreatedAt = await _ordersByUser.Where(x => x.UserId == userId && x.OrderId == orderId)
            .Select(x => x.CreatedAt).FirstOrDefault().ExecuteAsync();

        var batch = new BatchStatement()
            .Add(_ordersById.Where(x => x.OrderId == orderId)
                .Select(x => new OrdersById {Status = orderStatusDto.Status.GetName()}).Update())
            .Add(_ordersByUser.Where(x => x.UserId == userId && x.OrderId == orderId).Delete());
        await _session.ExecuteAsync(batch);

        await _session.ExecuteAsync(_ordersByUser.Insert(new OrdersByUser
        {
            OrderId = orderId, UserId = userId, Status = orderStatusDto.Status.GetName(),
            CreatedAt = orderCreatedAt, UpdatedAt = DateTime.Now
        }));
    }

    public async Task Delete(string id)
    {
        var userId = await _ordersById.Where(x => x.OrderId == Guid.Parse(id)).Select(x => x.UserId).FirstOrDefault()
            .ExecuteAsync();

        var batch = new BatchStatement()
            .Add(_ordersById.Where(x => x.OrderId == Guid.Parse(id)).Delete())
            .Add(_ordersByUser.Where(x => x.UserId == userId && x.OrderId == Guid.Parse(id)).Delete());
        await _session.ExecuteAsync(batch);
    }

    private static IEnumerable<ProductItemDto> GetItemsInOrder(IEnumerable<OrdersById> ordersById)
    {
        return ordersById.Select(orderById => new ProductItemDto
        {
            Product = new ProductDto
            {
                Id = orderById.ProductId.ToString(), Name = orderById.ProductName,
                Description = orderById.ProductDescription, Price = orderById.ProductPrice
            },
            Quantity = orderById.ProductQuantity
        });
    }

    private (PaymentDetailDto paymentDetails, AddressDto shippingAddress, ShippingMethodDto shippingMethod)
        GetDetailsInOrder(IEnumerable<OrdersById> ordersById)
    {
        var details = ordersById.Select(x => new
            {
                PaymentDetails = _mapper.Map<PaymentDetailDto>(x.PaymentDetails),
                ShippingAddress = _mapper.Map<AddressDto>(x.ShippingAddress),
                ShippingMethod = _mapper.Map<ShippingMethodDto>(x.ShippingMethod)
            })
            .FirstOrDefault();
        return (details!.PaymentDetails, details.ShippingAddress, details.ShippingMethod);
    }
}