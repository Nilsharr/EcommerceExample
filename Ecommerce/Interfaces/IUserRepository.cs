using Ecommerce.Dtos;
using Ecommerce.Enums;

namespace Ecommerce.Interfaces;

public interface IUserRepository : IGenericRepository<UserDto>
{
    Task<UserDto?> GetByEmail(string email);
    Task<bool> Exists(string email);
    Task<UserRole> GetRole(string id);
    Task<AddressDto> AddAddress(string userId, AddressDto address);
    Task DeleteAddress(string userId, string addressId);
    Task AddProductToCart(string userId, ProductIdItemDto productIdItem);
    Task DeleteProductFromCart(string userId, string productId);
}