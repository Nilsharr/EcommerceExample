using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Entities.Mongo;
using Ecommerce.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Ecommerce.Repositories.MongoRepositories;

public class MongoProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _collection;
    private readonly IMapper _mapper;

    public MongoProductRepository(IMongoDatabase database, IMapper mapper)
    {
        _collection = database.GetCollection<Product>("products");
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAll() => await GetAllByQuery();

    public async Task<IEnumerable<ProductDto>> GetAllByQuery(string? name = null, string[]? categories = null)
    {
        var productQuery = _collection.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
        {
            productQuery = productQuery.Where(x => x.Name.Contains(name));
        }

        if (categories is not null && categories.Length > 0)
        {
            // ReSharper disable once ConvertClosureToMethodGroup
            productQuery = productQuery.Where(x => x.Categories.Any(y => categories.Contains(y)));
        }

        return _mapper.Map<IEnumerable<ProductDto>>(await productQuery.ToListAsync());
    }

    public async Task<ProductDto?> GetById(string id)
    {
        return _mapper.Map<ProductDto?>(await _collection.AsQueryable().Where(x => x.Id == ObjectId.Parse(id))
            .SingleOrDefaultAsync());
    }

    public async Task<bool> Exists(string id)
    {
        return await _collection.AsQueryable().AnyAsync(x => x.Id == ObjectId.Parse(id));
    }

    public async Task<ProductDto> Add(ProductDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        await _collection.InsertOneAsync(product);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> Update(string id, ProductDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        await _collection.ReplaceOneAsync(x => x.Id == ObjectId.Parse(id), product);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task Delete(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == ObjectId.Parse(id));
    }
}