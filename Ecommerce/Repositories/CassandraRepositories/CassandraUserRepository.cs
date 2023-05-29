using AutoMapper;
using Cassandra;
using Ecommerce.Dtos;
using Ecommerce.Entities.Cassandra;
using Ecommerce.Enums;
using Ecommerce.Interfaces;
using ISession = Cassandra.ISession;
using Cassandra.Data.Linq;
using Ecommerce.Utils;
using Address = Ecommerce.Entities.Cassandra.Types.Address;

namespace Ecommerce.Repositories.CassandraRepositories;

// https://docs.datastax.com/en/developer/csharp-driver/3.19/
public class CassandraUserRepository : IUserRepository
{
    private readonly ISession _session;
    private readonly IMapper _mapper;
    private readonly Table<UsersById> _usersByIds;
    private readonly Table<UsersByEmail> _usersByEmail;
    private readonly Table<ProductsById> _productsById;
    private readonly Table<ProductsInUserCart> _productsInUserCart;

    public CassandraUserRepository(ISession session, IMapper mapper)
    {
        _session = session;
        _mapper = mapper;
        _usersByIds = new Table<UsersById>(_session);
        _usersByEmail = new Table<UsersByEmail>(_session);
        _productsById = new Table<ProductsById>(_session);
        _productsInUserCart = new Table<ProductsInUserCart>(_session);
    }

    public async Task<IEnumerable<UserDto>> GetAll()
    {
        return _mapper.Map<IEnumerable<UserDto>>(await _usersByIds.ExecuteAsync());
    }

    public async Task<UserDto?> GetById(string id)
    {
        var user = _mapper.Map<UserDto?>(await _usersByIds.FirstOrDefault(x => x.UserId == Guid.Parse(id))
            .ExecuteAsync());
        var productsInCart = await GetProductsInUserCart(user!.Id);
        user.ShoppingCart = productsInCart;
        return user;
    }

    public async Task<UserDto?> GetByEmail(string email)
    {
        return _mapper.Map<UserDto?>(await _usersByEmail.FirstOrDefault(x => x.Email == email).ExecuteAsync());
    }

    public async Task<UserRole> GetRole(string id)
    {
        var role = await _usersByIds.Where(x => x.UserId == Guid.Parse(id)).Select(x => x.Role)
            .FirstOrDefault().ExecuteAsync();
        return role.GetEnum<UserRole>();
    }

    public async Task<bool> Exists(string email)
    {
        // ReSharper disable once ReplaceWithSingleCallToCount
        return await _usersByEmail.Where(x => x.Email == email).Count().ExecuteAsync() > 0;
    }

    public async Task<UserDto> Add(UserDto userDto)
    {
        var guid = Guid.NewGuid();
        var userById = _mapper.Map<UsersById>(userDto);
        var userByEmail = _mapper.Map<UsersByEmail>(userDto);

        userById.UserId = guid;
        userByEmail.UserId = guid;

        var batch = new BatchStatement()
            .Add(_usersByIds.Insert(userById))
            .Add(_usersByEmail.Insert(userByEmail));
        await _session.ExecuteAsync(batch);

        return _mapper.Map<UserDto>(userById);
    }

    public async Task<AddressDto> AddAddress(string userId, AddressDto addressDto)
    {
        var addresses = await _usersByIds.Where(x => x.UserId == Guid.Parse(userId)).Select(x => x.Addresses)
            .FirstOrDefault().ExecuteAsync();
        var address = _mapper.Map<Address>(addressDto);
        address.Id = Guid.NewGuid();
        addresses.Add(address);

        await _usersByIds.Where(x => x.UserId == Guid.Parse(userId)).Select(x => new UsersById {Addresses = addresses})
            .Update().ExecuteAsync();
        return _mapper.Map<AddressDto>(address);
    }

    public async Task AddProductToCart(string userId, ProductIdItemDto productIdItem)
    {
        var product = await _productsById.FirstOrDefault(x => x.ProductId == Guid.Parse(productIdItem.ProductId))
            .ExecuteAsync();
        if (product is not null)
        {
            var productInCart = new ProductsInUserCart
            {
                UserId = Guid.Parse(userId),
                Quantity = productIdItem.Quantity,
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };
            await _productsInUserCart.Insert(productInCart).ExecuteAsync();
        }
    }

    public async Task<UserDto> Update(string id, UserDto userDto)
    {
        var batch = new BatchStatement()
            .Add(_usersByIds.Where(x => x.UserId == Guid.Parse(id)).Select(x => new UsersById
                {Role = userDto.Role.GetName(), Name = userDto.Name, Surname = userDto.Surname}).Update())
            .Add(_usersByEmail.Where(x => x.Email == userDto.Email)
                .Select(x => new UsersByEmail {Role = userDto.Role.GetName()}).Update());
        await _session.ExecuteAsync(batch);

        return userDto;
    }

    public async Task Delete(string id)
    {
        var email = await _usersByIds.Where(x => x.UserId == Guid.Parse(id)).Select(x => x.Email).FirstOrDefault()
            .ExecuteAsync();

        var batch = new BatchStatement()
            .Add(_usersByIds.Where(x => x.UserId == Guid.Parse(id)).Delete())
            .Add(_usersByEmail.Where(x => x.Email == email).Delete());
        await _session.ExecuteAsync(batch);
    }

    public async Task DeleteAddress(string userId, string addressId)
    {
        var addresses = await _usersByIds.Where(x => x.UserId == Guid.Parse(userId)).Select(x => x.Addresses)
            .FirstOrDefault().ExecuteAsync();

        await _usersByIds.Where(x => x.UserId == Guid.Parse(userId)).Select(x => new UsersById
            {Addresses = addresses.Where(z => z.Id != Guid.Parse(addressId)).ToList()}).Update().ExecuteAsync();
    }

    public async Task DeleteProductFromCart(string userId, string productId)
    {
        await _productsInUserCart.Where(x => x.UserId == Guid.Parse(userId) && x.ProductId == Guid.Parse(productId))
            .Delete().ExecuteAsync();
    }

    private async Task<IList<ProductItemDto>> GetProductsInUserCart(string userId)
    {
        var products = await _productsInUserCart.Where(x => x.UserId == Guid.Parse(userId)).ExecuteAsync();

        return products.Select(product => new ProductItemDto
        {
            Product = new ProductDto
            {
                Id = product.ProductId.ToString(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Image = product.Image
            },
            Quantity = product.Quantity
        }).ToList();
    }
}