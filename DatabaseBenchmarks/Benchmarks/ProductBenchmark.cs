using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Bogus;
using DatabaseBenchmarks.Utils;
using Ecommerce.Dtos;
using Ecommerce.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Perfolizer.Mathematics.OutlierDetection;

namespace DatabaseBenchmarks.Benchmarks;

[SimpleJob(RunStrategy.ColdStart, warmupCount: Constants.NumberOfWarmupIterations,
    iterationCount: Constants.NumberOfIterations)]
[Outliers(OutlierMode.RemoveAll)]
[MinColumn, MaxColumn, Q1Column, Q3Column]
[RPlotExporter]
public class ProductBenchmark
{
    private readonly IProductRepository _productRepository;
    private readonly Faker _faker;
    private readonly Faker<ProductDto> _fakeProductDto;
    private List<string> _productIds = default!;
    private int _index;

    public ProductBenchmark()
    {
        var provider = RegisterDatabases.Register();
        var repositoryFactory = provider.GetService<IRepositoryFactory>();

        _productRepository = repositoryFactory!.CreateProductRepository();
        _faker = new Faker();

        _fakeProductDto = new Faker<ProductDto>()
            .RuleFor(x => x.Name, f => f.Commerce.Product())
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.AmountInStock, f => f.Random.Number(1, 100))
            .RuleFor(x => x.Categories, f => f.PickRandom(Constants.ProductCategories, 3).ToList());
    }

    [GlobalSetup(Target = nameof(AddProduct))]
    public async Task Setup()
    {
        foreach (var id in (await _productRepository.GetAll()).Select(x => x.Id))
        {
            await _productRepository.Delete(id!);
        }
    }

    [GlobalSetup(Targets = new[] {nameof(GetProducts), nameof(UpdateProducts), nameof(DeleteProducts)})]
    public async Task SetupWithInsert()
    {
        _productIds = new List<string>();
        for (var i = 0; i < Constants.NumberOfIterations; i++)
        {
            _productIds.Add((await _productRepository.Add(_fakeProductDto.Generate())).Id!);
        }
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        foreach (var id in (await _productRepository.GetAll()).Select(x => x.Id))
        {
            await _productRepository.Delete(id!);
        }
    }

    [Benchmark]
    public async Task AddProduct() => await _productRepository.Add(_fakeProductDto.Generate());

    [Benchmark]
    public async Task GetProducts() => await _productRepository.GetAllByQuery(name: _faker.Commerce.Product());

    [Benchmark]
    public async Task UpdateProducts()
    {
        var product = _fakeProductDto.Generate();
        product.Id = _productIds[_index++];
        await _productRepository.Update(product.Id, product);
    }

    [Benchmark]
    public async Task DeleteProducts()
    {
        await _productRepository.Delete(_productIds[0]);
        _productIds.RemoveAt(0);
    }
}