using AutoMapper;
using Ecommerce.Entities.Postgres.Context;
using Ecommerce.Interfaces;
using Ecommerce.Repositories.CassandraRepositories;
using Ecommerce.Repositories.MongoRepositories;
using Ecommerce.Repositories.PostgresRepositories;
using MongoDB.Driver;
using ISession = Cassandra.ISession;

namespace Ecommerce.Factories;

public class RepositoryFactory<T> : IRepositoryFactory where T : class
{
    private readonly T _connection;
    private readonly IMapper _mapper;

    public RepositoryFactory(T connection, IMapper mapper)
    {
        _connection = connection;
        _mapper = mapper;
    }

    public IUserRepository CreateUserRepository()
    {
        return _connection switch
        {
            IMongoDatabase database => new MongoUserRepository(database, _mapper),
            ISession session => new CassandraUserRepository(session, _mapper),
            EcommerceDbContext dbContext => new PostgresUserRepository(dbContext, _mapper),
            _ => throw new ArgumentException("Invalid connection")
        };
    }

    public IProductRepository CreateProductRepository()
    {
        return _connection switch
        {
            IMongoDatabase database => new MongoProductRepository(database, _mapper),
            ISession session => new CassandraProductRepository(session, _mapper),
            EcommerceDbContext dbContext => new PostgresProductRepository(dbContext, _mapper),
            _ => throw new ArgumentException("Invalid connection")
        };
    }

    public IOrderRepository CreateOrderRepository()
    {
        return _connection switch
        {
            IMongoDatabase database => new MongoOrderRepository(database, _mapper),
            ISession session => new CassandraOrderRepository(session, _mapper),
            EcommerceDbContext dbContext => new PostgresOrderRepository(dbContext, _mapper),
            _ => throw new ArgumentException("Invalid connection")
        };
    }
}