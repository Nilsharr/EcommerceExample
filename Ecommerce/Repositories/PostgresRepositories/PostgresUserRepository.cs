using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Entities.Postgres;
using Ecommerce.Entities.Postgres.Context;
using Ecommerce.Enums;
using Ecommerce.Interfaces;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Ecommerce.Repositories.PostgresRepositories;

public class PostgresUserRepository : IUserRepository
{
    private readonly EcommerceDbContext _dbContext;
    private readonly IMapper _mapper;

    public PostgresUserRepository(EcommerceDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAll()
    {
        return _mapper.Map<IEnumerable<UserDto>>(await _dbContext.Users.Include(x => x.Addresses)
            .ThenInclude(x => x.Country).ToListAsync());
    }

    public async Task<UserDto?> GetById(string id)
    {
        return _mapper.Map<UserDto?>(await _dbContext.Users.Where(x => x.Id == Guid.Parse(id))
            .Include(x => x.ShoppingCartItems).ThenInclude(x => x.Product).ThenInclude(x => x.ProductCategories)
            .Include(x => x.Addresses)
            .ThenInclude(x => x.Country)
            .SingleOrDefaultAsync());
    }

    public async Task<UserDto?> GetByEmail(string email)
    {
        return _mapper.Map<UserDto?>(await _dbContext.Users.Where(x => x.Email == email).SingleOrDefaultAsync());
    }

    public async Task<UserRole> GetRole(string id)
    {
        return await _dbContext.Users.Where(x => x.Id == Guid.Parse(id)).Select(x => x.Role).SingleOrDefaultAsync();
    }

    public async Task<bool> Exists(string email)
    {
        return await _dbContext.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<UserDto> Add(UserDto userDto)
    {
        var inserted = _dbContext.Users.Add(_mapper.Map<User>(userDto)).Entity;
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<UserDto>(inserted);
    }

    public async Task<AddressDto> AddAddress(string userId, AddressDto addressDto)
    {
        // ReSharper disable once SpecifyStringComparison
        var country = await _dbContext.Countries.Where(x => x.Name.ToLower() == addressDto.Country.ToLower())
            .SingleOrDefaultAsync();
        var address = _mapper.Map<Address>(addressDto);
        if (country is not null)
        {
            address.Country = country;
        }

        address.UserId = Guid.Parse(userId);
        var inserted = _dbContext.Addresses.Add(address).Entity;
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<AddressDto>(inserted);
    }

    public async Task AddProductToCart(string userId, ProductIdItemDto productIdItem)
    {
        _dbContext.ShoppingCartItems.Add(new ShoppingCartItem
        {
            ProductId = Guid.Parse(productIdItem.ProductId), UserId = Guid.Parse(userId),
            Quantity = productIdItem.Quantity
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task<UserDto> Update(string id, UserDto userDto)
    {
        var updated = _dbContext.Users.Update(_mapper.Map<User>(userDto)).Entity;
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<UserDto>(updated);
    }

    public async Task Delete(string id)
    {
        await _dbContext.Users.Where(x => x.Id == Guid.Parse(id)).DeleteAsync();
    }

    public async Task DeleteAddress(string userId, string addressId)
    {
        await _dbContext.Addresses.Where(x => x.UserId == Guid.Parse(userId) && x.Id == Guid.Parse(addressId))
            .DeleteAsync();
    }

    public async Task DeleteProductFromCart(string userId, string productId)
    {
        await _dbContext.ShoppingCartItems
            .Where(x => x.UserId == Guid.Parse(userId) && x.ProductId == Guid.Parse(productId))
            .DeleteAsync();
    }
}