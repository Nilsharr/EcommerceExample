using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Bogus;
using Ecommerce.Dtos;
using Ecommerce.Enums;
using Ecommerce.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Perfolizer.Mathematics.OutlierDetection;

namespace DatabaseBenchmarks;

[SimpleJob(RunStrategy.ColdStart, warmupCount: Constants.NumberOfWarmupIterations,
    iterationCount: Constants.NumberOfIterations)]
[Outliers(OutlierMode.RemoveAll)]
[MinColumn, MaxColumn, Q1Column, Q3Column]
[RPlotExporter]
public class OrderBenchmark
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly Faker _faker;

    private List<string> _orderIds = default!;
    private List<string> _userIds = default!;
    private List<ProductDto> _products = default!;
    private readonly Dictionary<string, AddressDto> _addresses = new();
    private int _index;

    public OrderBenchmark()
    {
        var provider = RegisterDatabases.Register();
        var repositoryFactory = provider.GetService<IRepositoryFactory>();

        _orderRepository = repositoryFactory!.CreateOrderRepository();
        _userRepository = repositoryFactory.CreateUserRepository();
        _productRepository = repositoryFactory.CreateProductRepository();
        _faker = new Faker();
    }

    [GlobalSetup(Target = nameof(AddOrder))]
    public async Task InsertSetup()
    {
        foreach (var id in (await _orderRepository.GetAll()).Select(x => x.Id))
        {
            await _orderRepository.Delete(id!);
        }

        foreach (var id in (await _productRepository.GetAll()).Select(x => x.Id))
        {
            await _productRepository.Delete(id!);
        }

        foreach (var id in (await _userRepository.GetAll()).Select(x => x.Id))
        {
            await _userRepository.Delete(id);
        }

        _userIds = (await AddUsers()).ToList();
        _products = (await AddProducts()).ToList();
    }

    [GlobalSetup(Targets = new[]
        {nameof(SelectAllOrders), nameof(SelectAllOrdersWithStatus), nameof(UpdateOrder), nameof(DeleteOrder)})]
    public async Task SetupWithOrdersInsert()
    {
        _userIds = (await AddUsers()).ToList();
        _products = (await AddProducts()).ToList();

        _orderIds = new List<string>();
        for (var i = 0; i < Constants.NumberOfIterations; i++)
        {
            _orderIds.Add((await _orderRepository.Add(GenerateFakeOrder())).Id!);
        }
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        foreach (var id in (await _orderRepository.GetAll()).Select(x => x.Id))
        {
            await _orderRepository.Delete(id!);
        }

        foreach (var id in (await _productRepository.GetAll()).Select(x => x.Id))
        {
            await _productRepository.Delete(id!);
        }

        foreach (var id in (await _userRepository.GetAll()).Select(x => x.Id))
        {
            await _userRepository.Delete(id);
        }
    }

    [Benchmark]
    public async Task AddOrder() => await _orderRepository.Add(GenerateFakeOrder());

    [Benchmark]
    public async Task SelectAllOrders() => await _orderRepository.GetUserOrders(_userIds[_faker.Random.Number(0, 99)]);

    [Benchmark]
    public async Task SelectAllOrdersWithStatus() =>
        await _orderRepository.GetUserOrders(_userIds[_faker.Random.Number(0, 99)], _faker.PickRandom<OrderStatus>());

    [Benchmark]
    public async Task UpdateOrder()
    {
        var order = GenerateFakeOrder();
        order.Id = _orderIds[_index++];
        await _orderRepository.Update(order.Id, order);
    }

    [Benchmark]
    public async Task DeleteOrder()
    {
        await _orderRepository.Delete(_orderIds[0]);
        _orderIds.RemoveAt(0);
    }

    private async Task<IEnumerable<string>> AddUsers()
    {
        var fakeUser = new Faker<UserDto>()
            .RuleFor(x => x.Name, f => f.Name.FirstName())
            .RuleFor(x => x.Surname, f => f.Name.LastName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Salt, f => f.Random.Hash());

        var fakeAddress = new Faker<AddressDto>()
            .RuleFor(x => x.Street, f => f.Address.StreetName())
            .RuleFor(x => x.BuildingNumber, f => f.Address.BuildingNumber())
            .RuleFor(x => x.HouseNumber, f => f.Address.BuildingNumber())
            .RuleFor(x => x.PostalCode, f => f.Address.ZipCode())
            .RuleFor(x => x.City, f => f.Address.City())
            .RuleFor(x => x.Country, f => f.PickRandom(Constants.EuropeanCountries));

        var userIds = new List<string>();
        for (var i = 0; i < 100; i++)
        {
            var userId = (await _userRepository.Add(fakeUser.Generate())).Id;
            userIds.Add(userId);
            _addresses[userId] = await _userRepository.AddAddress(userId, fakeAddress.Generate());
        }

        return userIds;
    }

    private async Task<IEnumerable<ProductDto>> AddProducts()
    {
        var fakeProduct = new Faker<ProductDto>()
            .RuleFor(x => x.Name, f => f.Commerce.Product())
            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
            .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.AmountInStock, f => f.Random.Number(1, 100))
            .RuleFor(x => x.Categories, f => f.PickRandom(Constants.ProductCategories, 3).ToList());

        var products = new List<ProductDto>();
        for (var i = 0; i < 1000; i++)
        {
            products.Add(await _productRepository.Add(fakeProduct.Generate()));
        }

        return products;
    }

    private OrderDto GenerateFakeOrder()
    {
        var fakePaymentDetail = new Faker<PaymentDetailDto>()
            .RuleFor(x => x.NettoPrice, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.BruttoPrice, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.Tax, f => decimal.Parse(f.Commerce.Price()))
            .RuleFor(x => x.PaymentMethod, f => f.PickRandom(Constants.PaymentMethods))
            .RuleFor(x => x.Status, f => f.PickRandom(Constants.PaymentStatuses));

        var fakeShippingMethod = new Faker<ShippingMethodDto>()
            .RuleFor(x => x.Name, f => f.PickRandom(Constants.ShippingMethods))
            .RuleFor(x => x.Price, f => decimal.Parse(f.Commerce.Price()));

        var productItems = new List<ProductItemDto>
        {
            new() {Product = _products[_faker.Random.Number(0, 500)], Quantity = _faker.Random.Number(1, 10)},
            new() {Product = _products[_faker.Random.Number(501, 999)], Quantity = _faker.Random.Number(1, 10)},
        };
        productItems = productItems.GroupBy(x => x.Product).Select(x => x.First()).ToList();

        var randomUser = _faker.PickRandom(_userIds);
        var fakeOrderDto = new Faker<OrderDto>()
            .RuleFor(x => x.Status, f => f.PickRandom<OrderStatus>())
            .RuleFor(x => x.CreatedAt, _ => DateTime.Now)
            .RuleFor(x => x.UpdatedAt, _ => DateTime.Now)
            .RuleFor(x => x.PaymentDetails, _ => fakePaymentDetail.Generate())
            .RuleFor(x => x.ShippingAddress, _ => _addresses[randomUser])
            .RuleFor(x => x.ShippingMethod, _ => fakeShippingMethod.Generate())
            .RuleFor(x => x.Items, _ => productItems)
            .RuleFor(x => x.UserId, _ => randomUser);

        return fakeOrderDto.Generate();
    }
}