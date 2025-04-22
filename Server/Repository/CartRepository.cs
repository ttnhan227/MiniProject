using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Repository;

public class CartRepository : ICartRepository
{
    private readonly DatabaseContext _context;

    public CartRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task <IEnumerable<Cart>> GetByUser(string username)
    {
        return  _context.Carts
            .Include(c => c.Product)
            .Where(c => c.UserName == username)
            .ToList();
        
    }

    public async Task<Cart> AddToCart(Cart cart)
    {
       var existingCart =  await _context.Carts 
            .SingleOrDefaultAsync(c => c.UserName == cart.UserName && c.ProductId == cart.ProductId);

        if (existingCart != null)
        {
            existingCart.Quantity += cart.Quantity;
            _context.Update(existingCart);
        }
        else
        {
             _context.Carts.Add(cart);
        }

        _context.SaveChanges();
        return cart;
    }

    public async Task Payment(string username)
    {
        // Retrieve the list of cart items for the user
        var cartItems = await _context.Carts
            .Include(c => c.Product) // Ensure Product details are loaded
            .Where(c => c.UserName == username)
            .ToListAsync();

        if (cartItems == null || !cartItems.Any())
        {
            throw new Exception("No items found in the cart for the user.");
        }

        // Calculate the total price of the cart
        var totalPrice = cartItems.Sum(item => item.Quantity * (item.Product?.Price ?? 0));

        // Retrieve the user's account
        var acc = await _context.Accounts
            .SingleOrDefaultAsync(a => a.Username == username);

        if (acc == null)
        {
            throw new Exception("User account not found.");
        }

        // Update the user's total amount
        acc.TotalAmount += totalPrice;

        // Remove all items from the cart
        _context.Carts.RemoveRange(cartItems);

        // Save changes to the database
        await _context.SaveChangesAsync();

        ;
    }
   


}