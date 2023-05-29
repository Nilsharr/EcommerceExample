namespace Ecommerce.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(string id);
    Task<T> Add(T item);
    Task<T> Update(string id, T item);
    Task Delete(string id);
}