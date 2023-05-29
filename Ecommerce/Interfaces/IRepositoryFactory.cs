namespace Ecommerce.Interfaces;

public interface IRepositoryFactory
{
    IUserRepository CreateUserRepository();
    IOrderRepository CreateOrderRepository();
    IProductRepository CreateProductRepository();
}