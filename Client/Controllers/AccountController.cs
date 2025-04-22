using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Client.Controllers
{
    public class AccountController : Controller
    {
        private string uri = "https://localhost:7283/api/Account";
        private HttpClient client = new HttpClient();
        public IActionResult Index()
        {
            ViewBag.error = TempData["error"];
            return View();
        }

        [HttpPost]
        public IActionResult Index(AccountLogin accountLogin)
        {
            if (ModelState.IsValid)
            {
                var result = client.PostAsJsonAsync(uri, accountLogin).Result;
                if (result.IsSuccessStatusCode)
                {
                    var response = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                    var token = response["token"].ToString();
                    //luu token trong session
                    HttpContext.Session.SetString("token", token);
                    return Redirect("/Product/Index");
                }
            }
            ViewBag.error = "login fail!";
            return View();
        }
    }
}
