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
public class UserBenchmark
{
    private readonly IUserRepository _userRepository;
    private readonly Faker _faker;
    private readonly Faker<UserDto> _fakeUserDto;
    private List<string> _userIds = default!;

    public UserBenchmark()
    {
        var provider = RegisterDatabases.Register();
        var repositoryFactory = provider.GetService<IRepositoryFactory>();

        _userRepository = repositoryFactory!.CreateUserRepository();
        _faker = new Faker();

        _fakeUserDto = new Faker<UserDto>()
            .RuleFor(x => x.Name, f => f.Name.FirstName())
            .RuleFor(x => x.Surname, f => f.Name.LastName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Password, f => f.Internet.Password())
            .RuleFor(x => x.Salt, f => f.Random.Hash());
    }

    [GlobalSetup(Target = nameof(AddUser))]
    public async Task Setup()
    {
        foreach (var id in (await _userRepository.GetAll()).Select(x => x.Id))
        {
            await _userRepository.Delete(id);
        }
    }

    [GlobalSetup(Targets = new[] {nameof(EmailExists), nameof(DeleteUser)})]
    public async Task SetupWithInsert()
    {
        _userIds = new List<string>();
        for (var i = 0; i < Constants.NumberOfIterations; i++)
        {
            _userIds.Add((await _userRepository.Add(_fakeUserDto.Generate())).Id);
        }
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        foreach (var id in (await _userRepository.GetAll()).Select(x => x.Id))
        {
            await _userRepository.Delete(id);
        }
    }

    [Benchmark]
    public async Task AddUser() => await _userRepository.Add(_fakeUserDto.Generate());

    [Benchmark]
    public async Task EmailExists() => await _userRepository.Exists(_faker.Internet.Email());

    [Benchmark]
    public async Task DeleteUser()
    {
        await _userRepository.Delete(_userIds[0]);
        _userIds.RemoveAt(0);
    }
}