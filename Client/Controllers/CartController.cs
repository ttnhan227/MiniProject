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
            var token = HttpContext.Session.GetString("token");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;


            var result = client.GetStringAsync(uri + username).Result;
            var list = JsonConvert.DeserializeObject<IEnumerable<Cart>>(result);
            return View(list);
        }

        public IActionResult AddCart(int id)
        {
            var token = HttpContext.Session.GetString("token");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;

            Cart cart = new Cart
            {
                ProductId = id,
                Quantity = 1,
                UserName = username
            };

            var result = client.PostAsJsonAsync(uri, cart).Result;
            return RedirectToAction("Index");
        }

        public IActionResult Payment()
        {
            var token = HttpContext.Session.GetString("token");
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var username = jwtToken.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;

            var result = client.GetStreamAsync(uri + "payment/" + username).Result;
            return RedirectToAction("Index");

        }
    }
}
