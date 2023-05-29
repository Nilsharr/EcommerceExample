using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Entities.Postgres;
using Ecommerce.Entities.Postgres.Context;
using Ecommerce.Interfaces;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Ecommerce.Repositories.PostgresRepositories;

public class PostgresProductRepository : IProductRepository
{
    private readonly EcommerceDbContext _dbContext;
    private readonly IMapper _mapper;

    public PostgresProductRepository(EcommerceDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAll() => await GetAllByQuery();

    public async Task<IEnumerable<ProductDto>> GetAllByQuery(string? name = null, string[]? categories = null)
    {
        var productQuery = _dbContext.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
        {
            productQuery = productQuery.Where(x => x.Name.Contains(name));
        }

        if (categories is not null && categories.Length > 0)
        {
            productQuery = productQuery.Where(x => x.ProductCategories.Any(p => categories.Contains(p.Name)));
        }

        return _mapper.Map<IEnumerable<ProductDto>>(await productQuery.Include(x => x.ProductCategories).ToListAsync());
    }

    public async Task<ProductDto?> GetById(string id)
    {
        return _mapper.Map<ProductDto?>(await _dbContext.Products.Where(x => x.Id == Guid.Parse(id))
            .Include(x => x.ProductCategories).SingleOrDefaultAsync());
    }

    public async Task<bool> Exists(string id)
    {
        return await _dbContext.Products.AnyAsync(x => x.Id == Guid.Parse(id));
    }

    public async Task<ProductDto> Add(ProductDto productDto)
    {
        var categories = await _dbContext.ProductCategories
            .Where(x => productDto.Categories.Select(y => y.ToLower()).Contains(x.Name.ToLower()))
            .ToListAsync();

        var inserted = _dbContext.Products.Add(_mapper.Map<Product>(productDto)).Entity;
        inserted.ProductCategories = categories;
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<ProductDto>(inserted);
    }

    public async Task<ProductDto> Update(string id, ProductDto productDto)
    {
        var categories = await _dbContext.ProductCategories
            .Where(x => productDto.Categories.Select(y => y.ToLower()).Contains(x.Name.ToLower())).ToListAsync();

        var tracked = await _dbContext.Products.FindAsync(Guid.Parse(productDto.Id!));
        if (tracked is not null)
        {
            _dbContext.Entry(tracked).State = EntityState.Detached;
        }

        var updated = _dbContext.Products.Update(_mapper.Map<Product>(productDto)).Entity;

        await _dbContext.Entry(updated).Collection(x => x.ProductCategories).LoadAsync();

        foreach (var category in updated.ProductCategories.Where(x => categories.All(y => y.Name != x.Name)).ToList())
        {
            updated.ProductCategories.Remove(category);
        }

        foreach (var category in categories.Where(x => updated.ProductCategories.All(y => y.Name != x.Name)).ToList())
        {
            updated.ProductCategories.Add(category);
        }

        await _dbContext.SaveChangesAsync();
        return _mapper.Map<ProductDto>(updated);
    }

    public async Task Delete(string id)
    {
        await _dbContext.Products.Where(x => x.Id == Guid.Parse(id)).DeleteAsync();
    }
}