using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

namespace Client.Controllers
{
    public class CartController : Controller
    {
        private string uri = "https://localhost:7283/api/Cart/";
        private HttpClient client = new HttpClient();

        public IActionResult Index()
        {
            // Read token from cookies
            if (!HttpContext.Request.Cookies.TryGetValue("token", out string token) || string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username claim is missing in the token.");
                }

                // Add token to request header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Correct API endpoint
                HttpResponseMessage response = client.GetAsync(uri + "GetCart/" + username).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<IEnumerable<Cart>>(result);
                    return View(list);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Handle the case where the cart is not found (404)
                    return View(new List<Cart>()); // Pass an empty list to the view
                }
                else
                {
                    return StatusCode((int)response.StatusCode, $"An error occurred: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public IActionResult AddCart(int id)
        {
            // Read token from cookies
            if (!HttpContext.Request.Cookies.TryGetValue("token", out string token) || string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username claim is missing in the token.");
                }

                Cart cart = new Cart
                {
                    ProductId = id,
                    Quantity = 1,
                    UserName = username
                };

                // Add token to request header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Correct API endpoint
                var result = client.PostAsJsonAsync(uri + "AddToCart", cart).Result;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Make the action async
        public async Task<IActionResult> Payment() 
        {
            // Read token from cookies
            if (!HttpContext.Request.Cookies.TryGetValue("token", out string token) || string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest("Username claim is missing in the token.");
                }

                // Add token to request header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Correct API endpoint and HTTP method (POST instead of GET)
                // Use await and check the response
                var result = await client.PostAsync(uri + "Payment/" + username, null);

                if (result.IsSuccessStatusCode)
                {
                    // Fetch cart items
                    HttpResponseMessage cartResponse = await client.GetAsync(uri + "GetCart/" + username);
                    if (cartResponse.IsSuccessStatusCode)
                    {
                        var cartResult = await cartResponse.Content.ReadAsStringAsync();
                        var cartItems = JsonConvert.DeserializeObject<IEnumerable<Cart>>(cartResult);

                        // Calculate total amount
                        decimal totalAmount = 0;
                        foreach (var item in cartItems)
                        {
                            totalAmount += item.Quantity * 10; // Assuming each product has a fixed price for simplicity
                        }

                        // Simulate balance update
                        decimal currentBalance = 100; // Placeholder balance
                        decimal newBalance = currentBalance - totalAmount;

                        // Update success message
                        TempData["SuccessMessage"] = $"Total amount: {totalAmount}, your new balance: {newBalance}";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Payment processed successfully! Your cart has been cleared.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"Payment failed. Status code: {result.StatusCode}";
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
