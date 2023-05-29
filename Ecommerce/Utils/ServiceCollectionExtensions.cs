using AutoMapper;
using Cassandra;
using Ecommerce.Dtos;
using Ecommerce.Entities.Cassandra.Types;
using Ecommerce.Entities.Postgres.Context;
using Ecommerce.Factories;
using Ecommerce.Interfaces;
using Ecommerce.Settings;
using Ecommerce.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ISession = Cassandra.ISession;

namespace Ecommerce.Utils;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseAndRepositories(this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
        switch (databaseSettings.UsedDatabase.ToLowerInvariant())
        {
            case "mongo":
                services.AddSingleton<IMongoDatabase>(_ =>
                    new MongoClient(databaseSettings.Mongo.Connection).GetDatabase(databaseSettings.Mongo
                        .DatabaseName));

                services.AddSingleton<IRepositoryFactory>(serviceProvider =>
                {
                    var mongoDatabase = serviceProvider.GetService<IMongoDatabase>();
                    var mapper = serviceProvider.GetService<IMapper>();
                    return new RepositoryFactory<IMongoDatabase>(mongoDatabase!, mapper!);
                });
                break;
            case "cassandra":
                services.AddScoped<ISession>(_ =>
                {
                    var cluster = Cluster.Builder().AddContactPoint(databaseSettings.Cassandra.ContactPoint).Build();
                    return cluster.Connect(databaseSettings.Cassandra.Keyspace);
                });

                services.AddScoped<IRepositoryFactory>(serviceProvider =>
                {
                    var cassandraDatabase = serviceProvider.GetService<ISession>();
                    var mapper = serviceProvider.GetService<IMapper>();
                    MapCassandraTypes(cassandraDatabase!);
                    return new RepositoryFactory<ISession>(cassandraDatabase!, mapper!);
                });
                break;
            case "postgres":
                services.AddDbContextPool<EcommerceDbContext>(options =>
                    options.UseNpgsql(databaseSettings.Postgres.Connection).UseSnakeCaseNamingConvention());

                services.AddScoped<IRepositoryFactory>(serviceProvider =>
                {
                    var postgresDatabase = serviceProvider.GetService<EcommerceDbContext>();
                    var mapper = serviceProvider.GetService<IMapper>();
                    return new RepositoryFactory<EcommerceDbContext>(postgresDatabase!, mapper!);
                });
                break;
            default:
                throw new ArgumentException(
                    "Database type not supported. Supported values: mongo, cassandra, postgres.");
        }

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<UserLoginRequest>, UserLoginRequestValidator>();
        services.AddScoped<IValidator<UserRegisterRequest>, UserRegisterRequestValidator>();
        services.AddScoped<IValidator<AddressDto>, AddressValidator>();
        services.AddScoped<IValidator<ProductDto>, ProductValidator>();
        services.AddScoped<IValidator<OrderDto>, OrderValidator>();
        services.AddScoped<IValidator<ProductIdItemDto>, ProductIdItemValidator>();
        return services;
    }

    private static void MapCassandraTypes(ISession session)
    {
        session.UserDefinedTypes.Define(
            UdtMap.For<Address>("address")
                .Map(x => x.Id, "id")
                .Map(x => x.Street, "street")
                .Map(x => x.BuildingNumber, "building_number")
                .Map(x => x.HouseNumber, "house_number")
                .Map(x => x.PostalCode, "postal_code")
                .Map(x => x.City, "city")
                .Map(x => x.Country, "country"),
            UdtMap.For<PaymentDetail>("payment_detail")
                .Map(x => x.NettoPrice, "netto_price")
                .Map(x => x.BruttoPrice, "brutto_price")
                .Map(x => x.Tax, "tax")
                .Map(x => x.PaymentMethod, "payment_method")
                .Map(x => x.Status, "status"),
            UdtMap.For<ShippingMethod>("shipping_method")
                .Map(x => x.Name, "name")
                .Map(x => x.Price, "price")
        );
    }
}