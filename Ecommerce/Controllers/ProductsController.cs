using Ecommerce.Dtos;
using Ecommerce.Filters;
using Ecommerce.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<ProductDto> _productValidator;

    public ProductsController(IRepositoryFactory repositoryFactory, IValidator<ProductDto> productValidator)
    {
        _productRepository = repositoryFactory.CreateProductRepository();
        _productValidator = productValidator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] string? name = null,
        [FromQuery] string[]? categories = null)
    {
        var products = await _productRepository.GetAllByQuery(name, categories);
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> Get(string id)
    {
        var product = await _productRepository.GetById(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<ProductDto>> Add(ProductDto product)
    {
        var validationResult = await _productValidator.ValidateAsync(product);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var inserted = await _productRepository.Add(product);
        return CreatedAtAction(nameof(Get), new {inserted.Id}, inserted);
    }

    [HttpPut("{id}")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult<ProductDto>> Update(string id, ProductDto product)
    {
        var validationResult = await _productValidator.ValidateAsync(product);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        if (product.Id != id)
        {
            return BadRequest();
        }

        if (await _productRepository.Exists(id))
        {
            var updated = await _productRepository.Update(id, product);
            return Ok(updated);
        }

        var inserted = await _productRepository.Add(product);
        return CreatedAtAction(nameof(Get), new {inserted.Id}, inserted);
    }

    [HttpDelete("{id}")]
    [TypeFilter(typeof(AdminAuthorizationFilter))]
    public async Task<ActionResult> Delete(string id)
    {
        await _productRepository.Delete(id);
        return NoContent();
    }
}