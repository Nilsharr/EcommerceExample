using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Entities.Postgres;
using Ecommerce.Entities.Postgres.Context;
using Ecommerce.Enums;
using Ecommerce.Interfaces;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
using OrderStatus = Ecommerce.Enums.OrderStatus;

namespace Ecommerce.Repositories.PostgresRepositories;

[SuppressMessage("ReSharper", "SpecifyStringComparison")]
public class PostgresOrderRepository : IOrderRepository
{
    private readonly EcommerceDbContext _dbContext;
    private readonly IMapper _mapper;

    public PostgresOrderRepository(EcommerceDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDto>> GetAll()
    {
        return _mapper.Map<IEnumerable<OrderDto>>(await _dbContext.Orders.Include(x => x.OrderStatus)
            .Include(x => x.PaymentDetails).ThenInclude(x => x.PaymentMethod).Include(x => x.PaymentDetails)
            .ThenInclude(x => x.PaymentStatus).Include(x => x.ShippingAddress).ThenInclude(x => x.Country)
            .Include(x => x.ShippingMethod).Include(x => x.OrderItems).ThenInclude(x => x.Product.ProductCategories)
            .ToListAsync());
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrders(string userId, OrderStatus? status = null)
    {
        var orderQuery = _dbContext.Orders.AsQueryable().Where(x => x.UserId == Guid.Parse(userId));
        if (status is not null)
        {
            orderQuery = orderQuery.Where(x => x.OrderStatus.Name == status.Value.GetName());
        }

        return _mapper.Map<IEnumerable<OrderDto>>(await orderQuery.Include(x => x.OrderStatus)
            .Include(x => x.PaymentDetails).ThenInclude(x => x.PaymentMethod).Include(x => x.PaymentDetails)
            .ThenInclude(x => x.PaymentStatus).Include(x => x.ShippingAddress).ThenInclude(x => x.Country)
            .Include(x => x.ShippingMethod).Include(x => x.OrderItems).ThenInclude(x => x.Product.ProductCategories)
            .ToListAsync());
    }

    public async Task<OrderDto?> GetById(string id)
    {
        return _mapper.Map<OrderDto?>(await _dbContext.Orders.Where(x => x.Id == Guid.Parse(id))
            .Include(x => x.OrderStatus)
            .Include(x => x.PaymentDetails).ThenInclude(x => x.PaymentMethod).Include(x => x.PaymentDetails)
            .ThenInclude(x => x.PaymentStatus).Include(x => x.ShippingAddress).ThenInclude(x => x.Country)
            .Include(x => x.ShippingMethod).Include(x => x.OrderItems).ThenInclude(x => x.Product.ProductCategories)
            .SingleOrDefaultAsync());
    }

    public async Task<bool> Exists(string id)
    {
        return await _dbContext.Orders.AnyAsync(x => x.Id == Guid.Parse(id));
    }

    public async Task<OrderDto> Add(OrderDto orderDto)
    {
        foreach (var insertedOrderItem in orderDto.Items)
        {
            var tracked = await _dbContext.Products.FindAsync(Guid.Parse(insertedOrderItem.Product.Id!));
            if (tracked is not null)
            {
                _dbContext.Entry(tracked).State = EntityState.Detached;
            }
        }

        var trackedAddress = await _dbContext.Addresses.FindAsync(Guid.Parse(orderDto.ShippingAddress.Id!));
        if (trackedAddress is not null)
        {
            _dbContext.Entry(trackedAddress).State = EntityState.Detached;
        }

        var paymentMethod = await _dbContext.PaymentMethods
            .Where(x => x.Name.ToLower() == orderDto.PaymentDetails.PaymentMethod.ToLower()).SingleOrDefaultAsync();
        var paymentStatus = await _dbContext.PaymentStatuses
            .Where(x => x.Name.ToLower() == orderDto.PaymentDetails.Status.ToLower()).SingleOrDefaultAsync();

        var inserted = _dbContext.Orders.Add(_mapper.Map<Order>(orderDto)).Entity;
        inserted.PaymentDetails.PaymentMethod = paymentMethod ?? throw new InvalidOperationException();
        inserted.PaymentDetails.PaymentStatus = paymentStatus ?? throw new InvalidOperationException();

        _dbContext.Attach(inserted.ShippingAddress);
        await _dbContext.Entry(inserted.ShippingAddress).Reference(x => x.Country).LoadAsync();

        foreach (var insertedOrderItem in inserted.OrderItems)
        {
            _dbContext.Attach(insertedOrderItem.Product);
            await _dbContext.Entry(insertedOrderItem.Product).Collection(x => x.ProductCategories).LoadAsync();
        }

        await _dbContext.SaveChangesAsync();
        return _mapper.Map<OrderDto>(inserted);
    }

    public async Task<OrderDto> Update(string id, OrderDto orderDto)
    {
        var paymentMethod = await _dbContext.PaymentMethods
            .Where(x => x.Name.ToLower() == orderDto.PaymentDetails.PaymentMethod.ToLower()).SingleOrDefaultAsync();
        var paymentStatus = await _dbContext.PaymentStatuses
            .Where(x => x.Name.ToLower() == orderDto.PaymentDetails.Status.ToLower()).SingleOrDefaultAsync();

        var tracked = await _dbContext.Orders.FindAsync(Guid.Parse(orderDto.Id!));
        if (tracked is not null)
        {
            _dbContext.Entry(tracked).State = EntityState.Detached;
        }

        foreach (var insertedOrderItem in orderDto.Items)
        {
            var trackedProduct = await _dbContext.Products.FindAsync(Guid.Parse(insertedOrderItem.Product.Id!));
            if (trackedProduct is not null)
            {
                _dbContext.Entry(trackedProduct).State = EntityState.Detached;
            }

            var trackedOrderItem = await _dbContext.OrderItems.FindAsync(Guid.Parse(orderDto.Id!),
                Guid.Parse(insertedOrderItem.Product.Id!));
            if (trackedOrderItem is not null)
            {
                _dbContext.Entry(trackedOrderItem).State = EntityState.Detached;
            }
        }

        var trackedAddress = await _dbContext.Addresses.FindAsync(Guid.Parse(orderDto.ShippingAddress.Id!));
        if (trackedAddress is not null)
        {
            _dbContext.Entry(trackedAddress).State = EntityState.Detached;
        }

        var updated = _dbContext.Orders.Update(_mapper.Map<Order>(orderDto)).Entity;
        updated.PaymentDetails.PaymentMethod = paymentMethod ?? throw new InvalidOperationException();
        updated.PaymentDetails.PaymentStatus = paymentStatus ?? throw new InvalidOperationException();
        _dbContext.Attach(updated.ShippingAddress);
        await _dbContext.Entry(updated.ShippingAddress).Reference(x => x.Country).LoadAsync();
        _dbContext.AttachRange(updated.OrderItems);

        foreach (var updatedOrderItem in updated.OrderItems)
        {
            _dbContext.Attach(updatedOrderItem.Product);
            await _dbContext.Entry(updatedOrderItem.Product).Collection(x => x.ProductCategories).LoadAsync();
        }

        await _dbContext.SaveChangesAsync();
        return _mapper.Map<OrderDto>(updated);
    }

    public async Task UpdateStatus(string id, OrderStatusDto orderStatus)
    {
        var statusId = await _dbContext.OrderStatuses
            .Where(x => x.Name.ToLower() == orderStatus.Status.GetName().ToLower()).Select(x => x.Id)
            .SingleOrDefaultAsync();
        if (statusId != 0)
        {
            await _dbContext.Orders.Where(x => x.Id == Guid.Parse(id))
                .UpdateAsync(x => new Order {OrderStatusId = statusId, UpdatedAt = DateTime.Now});
        }
    }

    public async Task Delete(string id)
    {
        await _dbContext.Orders.Where(x => x.Id == Guid.Parse(id)).DeleteAsync();
    }
}