using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Entities.Mongo;
using Ecommerce.Entities.Mongo.Types;
using Ecommerce.Enums;
using Ecommerce.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Ecommerce.Repositories.MongoRepositories;

// https://www.mongodb.com/docs/drivers/csharp/current/usage-examples/
public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;
    private readonly IMongoCollection<Product> _productCollection;
    private readonly IMapper _mapper;

    public MongoUserRepository(IMongoDatabase database, IMapper mapper)
    {
        _collection = database.GetCollection<User>("users");
        _productCollection = database.GetCollection<Product>("products");
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAll()
    {
        return _mapper.Map<IEnumerable<UserDto>>(await _collection.AsQueryable().ToListAsync());
    }

    public async Task<UserDto?> GetById(string id)
    {
        return _mapper.Map<UserDto?>(await _collection.AsQueryable().Where(x => x.Id == ObjectId.Parse(id))
            .SingleOrDefaultAsync());
    }

    public async Task<UserDto?> GetByEmail(string email)
    {
        return _mapper.Map<UserDto?>(await _collection.AsQueryable().Where(x => x.Email == email)
            .SingleOrDefaultAsync());
    }

    public async Task<UserRole> GetRole(string id)
    {
        return await _collection.AsQueryable().Where(x => x.Id == ObjectId.Parse(id)).Select(x => x.Role)
            .SingleOrDefaultAsync();
    }

    public async Task<bool> Exists(string email)
    {
        return await _collection.AsQueryable().AnyAsync(x => x.Email == email);
    }

    public async Task<UserDto> Add(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        await _collection.InsertOneAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<AddressDto> AddAddress(string userId, AddressDto addressDto)
    {
        var address = _mapper.Map<Address>(addressDto);
        address.Id = ObjectId.GenerateNewId();
        var update = Builders<User>.Update.Push(x => x.Addresses, address);
        await _collection.UpdateOneAsync(x => x.Id == ObjectId.Parse(userId), update);
        return _mapper.Map<AddressDto>(address);
    }

    public async Task AddProductToCart(string userId, ProductIdItemDto productIdItem)
    {
        var product = await _productCollection.AsQueryable().Where(x => x.Id == ObjectId.Parse(productIdItem.ProductId))
            .SingleOrDefaultAsync();
        if (product is not null)
        {
            var update = Builders<User>.Update.Push(x => x.ShoppingCart,
                new ProductItem {Quantity = productIdItem.Quantity, Product = product});
            await _collection.UpdateOneAsync(x => x.Id == ObjectId.Parse(userId), update);
        }
    }

    public async Task<UserDto> Update(string id, UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        await _collection.ReplaceOneAsync(x => x.Id == ObjectId.Parse(id), user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task Delete(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == ObjectId.Parse(id));
    }

    public async Task DeleteAddress(string userId, string addressId)
    {
        var update = Builders<User>.Update.PullFilter(x => x.Addresses, address => address.Id ==
            ObjectId.Parse(addressId));
        await _collection.UpdateOneAsync(x => x.Id == ObjectId.Parse(userId), update);
    }

    public async Task DeleteProductFromCart(string userId, string productId)
    {
        var update = Builders<User>.Update.PullFilter(x => x.ShoppingCart,
            productItem => productItem.Product.Id == ObjectId.Parse(productId));
        await _collection.UpdateOneAsync(x => x.Id == ObjectId.Parse(userId), update);
    }
}