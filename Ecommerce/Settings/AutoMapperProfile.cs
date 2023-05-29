using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Enums;
using Ecommerce.Utils;
using MongoDB.Bson;

namespace Ecommerce.Settings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreatePostgresMapping();
        CreateMongoMapping();
        CreateCassandraMapping();
    }

    private void CreatePostgresMapping()
    {
        CreateMap<Entities.Postgres.Address, AddressDto>()
            .ForMember(x => x.Country, opt => opt.MapFrom(x => x.Country.Name));
        CreateMap<AddressDto, Entities.Postgres.Address>()
            .ForMember(x => x.Country, opt => opt.Ignore());

        CreateMap<Entities.Postgres.PaymentDetail, PaymentDetailDto>();
        CreateMap<PaymentDetailDto, Entities.Postgres.PaymentDetail>();

        CreateMap<Entities.Postgres.ShippingMethod, ShippingMethodDto>();
        CreateMap<ShippingMethodDto, Entities.Postgres.ShippingMethod>();

        CreateMap<Entities.Postgres.Product, ProductDto>()
            .ForMember(x => x.Categories, opt => opt.MapFrom(x => x.ProductCategories.Select(y => y.Name)));
        CreateMap<ProductDto, Entities.Postgres.Product>();

        CreateMap<Entities.Postgres.ShoppingCartItem, ProductItemDto>();
        CreateMap<ProductItemDto, Entities.Postgres.ShoppingCartItem>();

        CreateMap<Entities.Postgres.User, UserDto>()
            .ForMember(x => x.ShoppingCart, opt => opt.MapFrom(x => x.ShoppingCartItems));
        CreateMap<UserDto, Entities.Postgres.User>();

        CreateMap<Entities.Postgres.PaymentDetail, PaymentDetailDto>()
            .ForMember(x => x.PaymentMethod, opt => opt.MapFrom(x => x.PaymentMethod.Name))
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.PaymentStatus.Name));
        CreateMap<PaymentDetailDto, Entities.Postgres.PaymentDetail>()
            .ForMember(x => x.PaymentMethod, opt => opt.Ignore())
            .ForMember(x => x.PaymentStatus, opt => opt.Ignore());

        CreateMap<Entities.Postgres.ShippingMethod, ShippingMethodDto>();
        CreateMap<ShippingMethodDto, Entities.Postgres.ShippingMethod>();

        CreateMap<Entities.Postgres.OrderItem, ProductItemDto>();
        CreateMap<ProductItemDto, Entities.Postgres.OrderItem>();

        CreateMap<Entities.Postgres.Order, OrderDto>()
            .ForMember(x => x.Status, opt => opt.MapFrom(x => x.OrderStatus.Name.GetEnum<OrderStatus>(true)))
            .ForMember(x => x.Items, opt => opt.MapFrom(x => x.OrderItems));
        CreateMap<OrderDto, Entities.Postgres.Order>()
            .ForMember(x => x.OrderStatus,
                opt => opt.MapFrom(x => new Entities.Postgres.OrderStatus {Name = x.Status!.Value.GetName()}))
            .ForMember(x => x.OrderItems, opt => opt.MapFrom(x => x.Items));
    }

    private void CreateMongoMapping()
    {
        CreateMap<Entities.Mongo.User, UserDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.ToString()));
        CreateMap<UserDto, Entities.Mongo.User>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => ObjectId.Parse(x.Id)));

        CreateMap<Entities.Mongo.Types.Address, AddressDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.ToString()));
        CreateMap<AddressDto, Entities.Mongo.Types.Address>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => ObjectId.Parse(x.Id)));

        CreateMap<Entities.Mongo.Order, OrderDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.ToString()))
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.UserId.ToString()));
        CreateMap<OrderDto, Entities.Mongo.Order>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => ObjectId.Parse(x.Id)))
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => ObjectId.Parse(x.UserId)));

        CreateMap<Entities.Mongo.Product, ProductDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.ToString()));
        CreateMap<ProductDto, Entities.Mongo.Product>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => ObjectId.Parse(x.Id)));

        CreateMap<Entities.Mongo.Types.ProductItem, ProductItemDto>();
        CreateMap<ProductItemDto, Entities.Mongo.Types.ProductItem>();

        CreateMap<Entities.Mongo.Types.PaymentDetail, PaymentDetailDto>();
        CreateMap<PaymentDetailDto, Entities.Mongo.Types.PaymentDetail>();

        CreateMap<Entities.Mongo.Types.ShippingMethod, ShippingMethodDto>();
        CreateMap<ShippingMethodDto, Entities.Mongo.Types.ShippingMethod>();
    }

    private void CreateCassandraMapping()
    {
        CreateMap<Entities.Cassandra.Types.Address, AddressDto>();
        CreateMap<AddressDto, Entities.Cassandra.Types.Address>();

        CreateMap<Entities.Cassandra.Types.PaymentDetail, PaymentDetailDto>();
        CreateMap<PaymentDetailDto, Entities.Cassandra.Types.PaymentDetail>();

        CreateMap<Entities.Cassandra.Types.ShippingMethod, ShippingMethodDto>();
        CreateMap<ShippingMethodDto, Entities.Cassandra.Types.ShippingMethod>();

        CreateMap<Entities.Cassandra.UsersById, UserDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.UserId));
        CreateMap<UserDto, Entities.Cassandra.UsersById>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.Id));

        CreateMap<Entities.Cassandra.UsersByEmail, UserDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.UserId));
        CreateMap<UserDto, Entities.Cassandra.UsersByEmail>()
            .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.Id));

        CreateMap<Entities.Cassandra.ProductsById, ProductDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ProductId));
        CreateMap<ProductDto, Entities.Cassandra.ProductsById>()
            .ForMember(x => x.ProductId, opt => opt.MapFrom(x => x.Id));

        CreateMap<Entities.Cassandra.ProductsByName, ProductDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.ProductId));
        CreateMap<ProductDto, Entities.Cassandra.ProductsByName>()
            .ForMember(x => x.ProductId, opt => opt.MapFrom(x => x.Id));

        CreateMap<Entities.Cassandra.OrdersById, OrderDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.OrderId));
        CreateMap<OrderDto, Entities.Cassandra.OrdersById>()
            .ForMember(x => x.OrderId, opt => opt.MapFrom(x => x.Id));

        CreateMap<Entities.Cassandra.OrdersByUser, OrderDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.OrderId));
        CreateMap<OrderDto, Entities.Cassandra.OrdersByUser>()
            .ForMember(x => x.OrderId, opt => opt.MapFrom(x => x.Id));
    }
}