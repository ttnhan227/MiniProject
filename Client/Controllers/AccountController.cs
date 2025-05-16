using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;

namespace Client.Controllers
{
    public class AccountController : Controller
    {
        private string uri = "https://localhost:7288/api/Account";
        private HttpClient client = new HttpClient();

        public IActionResult Index()
        {
            if (HttpContext.Request.Cookies.ContainsKey("token"))
            {
                return Redirect("/Home/Index");
            }
            ViewBag.error = TempData["error"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AccountLogin accountLogin)
        {
            if (ModelState.IsValid)
            {
                var result = client.PostAsJsonAsync(uri, accountLogin).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                    var token = response["token"].ToString();

                    // Parse the JWT token
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    // Create claims from the token
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, accountLogin.Username),
                        new Claim(ClaimTypes.Role, jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    // Sign in the user
                    await HttpContext.SignInAsync("Cookies", claimsPrincipal);

                    // Store the JWT token
                    HttpContext.Response.Cookies.Append("token", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                    return Redirect("/Home/Index");
                }
            }
            ViewBag.error = "Login failed!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");    // Xóa ClaimsPrincipal
            HttpContext.Response.Cookies.Delete("token"); // Xóa token lưu trong cookie
            return RedirectToAction("Index");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
