// ICartRepository.cs
using Server.Models;

public interface ICartRepository
{
    Task<IEnumerable<Cart>> GetByUser(string username);
    Task<Cart> AddToCart(Cart cart);
    Task Payment(string username); // payment
    Task RemoveProductFromAllCarts(int productId);
}
