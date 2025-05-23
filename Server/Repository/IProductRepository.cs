using Server.Models;

public interface IProductRepository
{
    Task<List<Product>> GetAll();
    Task<Product> GetById(int id);
    Task Add(Product product);
    Task Update(Product product);
    Task<List<Product>> Search(string searchTerm);
    Task Delete(int id);
}
