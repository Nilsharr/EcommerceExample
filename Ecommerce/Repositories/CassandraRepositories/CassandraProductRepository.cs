using AutoMapper;
using Cassandra;
using Cassandra.Data.Linq;
using Ecommerce.Dtos;
using Ecommerce.Entities.Cassandra;
using Ecommerce.Interfaces;
using ISession = Cassandra.ISession;

namespace Ecommerce.Repositories.CassandraRepositories;

public class CassandraProductRepository : IProductRepository
{
    private readonly ISession _session;
    private readonly IMapper _mapper;
    private readonly Table<ProductsById> _productsById;
    private readonly Table<ProductsByName> _productsByName;

    public CassandraProductRepository(ISession session, IMapper mapper)
    {
        _session = session;
        _mapper = mapper;
        _productsById = new Table<ProductsById>(_session);
        _productsByName = new Table<ProductsByName>(_session);
    }

    public async Task<IEnumerable<ProductDto>> GetAll() => await GetAllByQuery();

    public async Task<IEnumerable<ProductDto>> GetAllByQuery(string? name = null, string[]? categories = null)
    {
        CqlQuery<ProductsByName> productQuery = _productsByName;
        if (!string.IsNullOrWhiteSpace(name))
        {
            productQuery = productQuery.Where(x => x.Name == name);
        }

        if (categories is not null && categories.Length > 0)
        {
            throw new NotImplementedException();
            //productQuery = productQuery.Where(x => x.Categories.Any(y => categories.Contains(y)));
        }

        return _mapper.Map<IEnumerable<ProductDto>>(await productQuery.ExecuteAsync());
    }

    public async Task<ProductDto?> GetById(string id)
    {
        return _mapper.Map<ProductDto?>(await _productsById.FirstOrDefault(x => x.ProductId == Guid.Parse(id))
            .ExecuteAsync());
    }

    public async Task<bool> Exists(string id)
    {
        // ReSharper disable once ReplaceWithSingleCallToCount
        return await _productsById.Where(x => x.ProductId == Guid.Parse(id)).Count().ExecuteAsync() > 0;
    }

    public async Task<ProductDto> Add(ProductDto productDto)
    {
        var guid = Guid.NewGuid();
        var productById = _mapper.Map<ProductsById>(productDto);
        var productByName = _mapper.Map<ProductsByName>(productDto);

        productById.ProductId = guid;
        productByName.ProductId = guid;

        var batch = new BatchStatement()
            .Add(_productsById.Insert(productById))
            .Add(_productsByName.Insert(productByName));
        await _session.ExecuteAsync(batch);

        return _mapper.Map<ProductDto>(productById);
    }

    public async Task<ProductDto> Update(string id, ProductDto productDto)
    {
        var productById = await _productsById.FirstOrDefault(x => x.ProductId == Guid.Parse(id)).ExecuteAsync();
        var batch = new BatchStatement()
            .Add(_productsById.Where(x => x.ProductId == Guid.Parse(id)).Select(x => new ProductsById
            {
                Description = productDto.Description, Price = productDto.Price,
                AmountInStock = productDto.AmountInStock, Categories = productDto.Categories, Image = productDto.Image!
            }).Update())
            .Add(_productsByName.Where(x => x.Name == productById.Name && x.ProductId == productById.ProductId).Select(
                x => new ProductsByName
                {
                    Description = productDto.Description, Price = productDto.Price,
                    AmountInStock = productDto.AmountInStock, Categories = productDto.Categories,
                    Image = productDto.Image!
                }).Update());
        await _session.ExecuteAsync(batch);

        return productDto;
    }

    public async Task Delete(string id)
    {
        var name = await _productsById.Where(x => x.ProductId == Guid.Parse(id)).Select(x => x.Name).FirstOrDefault()
            .ExecuteAsync();

        var batch = new BatchStatement()
            .Add(_productsById.Where(x => x.ProductId == Guid.Parse(id)).Delete())
            .Add(_productsByName.Where(x => x.Name == name && x.ProductId == Guid.Parse(id)).Delete());
        await _session.ExecuteAsync(batch);
    }
}