using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Entities.Mongo;
using Ecommerce.Enums;
using Ecommerce.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Ecommerce.Repositories.MongoRepositories;

public class MongoOrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _collection;
    private readonly IMongoCollection<User> _usersCollection;
    private readonly IMapper _mapper;

    public MongoOrderRepository(IMongoDatabase database, IMapper mapper)
    {
        _collection = database.GetCollection<Order>("orders");
        _usersCollection = database.GetCollection<User>("users");
        _mapper = mapper;
    }

    public async Task<IEnumerable<OrderDto>> GetAll()
    {
        return _mapper.Map<IEnumerable<OrderDto>>(await _collection.AsQueryable().ToListAsync());
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrders(string userId, OrderStatus? status = null)
    {
        var orderQuery = _collection.AsQueryable().Where(x => x.UserId == ObjectId.Parse(userId));
        if (status is not null)
        {
            orderQuery = orderQuery.Where(x => x.Status == status);
        }

        return _mapper.Map<IEnumerable<OrderDto>>(await orderQuery.ToListAsync());
    }

    public async Task<OrderDto?> GetById(string id)
    {
        return _mapper.Map<OrderDto?>(await _collection.AsQueryable().Where(x => x.Id == ObjectId.Parse(id))
            .SingleOrDefaultAsync());
    }

    public async Task<bool> Exists(string id)
    {
        return await _collection.AsQueryable().AnyAsync(x => x.Id == ObjectId.Parse(id));
    }

    public async Task<OrderDto> Add(OrderDto orderDto)
    {
        var order = _mapper.Map<Order>(orderDto);
        await _collection.InsertOneAsync(order);

        await _usersCollection.UpdateOneAsync(x => x.Id == order.UserId,
            Builders<User>.Update.Push(u => u.OrderIds, order.Id));
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<OrderDto> Update(string id, OrderDto orderDto)
    {
        var order = _mapper.Map<Order>(orderDto);
        order.UpdatedAt = DateTime.Now;
        await _collection.ReplaceOneAsync(x => x.Id == ObjectId.Parse(id), order);
        return _mapper.Map<OrderDto>(order);
    }

    public async Task UpdateStatus(string id, OrderStatusDto orderStatus)
    {
        var update = Builders<Order>.Update.Set(x => x.Status, orderStatus.Status).Set(x => x.UpdatedAt, DateTime.Now);
        await _collection.UpdateOneAsync(x => x.Id == ObjectId.Parse(id), update);
    }

    public async Task Delete(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == ObjectId.Parse(id));
    }
}