using Server.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "user")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        // Get all items in the user's cart
        [HttpGet("GetCart/{username}")]
        public async Task<IActionResult> GetCart(string username)
        {
            try
            {
                var cartItems = await _cartRepository.GetByUser(username);
                if (cartItems == null || !cartItems.Any())
                {
                    return NotFound("No items in the cart.");
                }
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Add a product to the user's cart
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] Cart cart)
        {
            if (cart == null)
            {
                return BadRequest("Cart item is null.");
            }

            try
            {
                var addedCart = await _cartRepository.AddToCart(cart);
                return Ok(new { Message = "Product added to cart successfully.", Cart = addedCart });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Clear all items in the user's cart and process payment
        [HttpPost("Payment/{username}")]
        public async Task<IActionResult> Payment(string username)
        {
            try
            {
                await _cartRepository.Payment(username);
                return Ok(new { Message = "Payment processed successfully."});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}