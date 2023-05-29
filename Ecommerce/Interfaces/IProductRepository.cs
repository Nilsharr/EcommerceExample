using Ecommerce.Dtos;

namespace Ecommerce.Interfaces;

public interface IProductRepository : IGenericRepository<ProductDto>
{
    Task<bool> Exists(string id);

    Task<IEnumerable<ProductDto>> GetAllByQuery(string? name = null, string[]? categories = null);
}