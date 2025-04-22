using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class ProductController : Controller
    {
        private string uri = "https://localhost:7283/api/Product/";
        private HttpClient client = new HttpClient();

        [NonAction]
        private void AddHeader()
        {
            var token = HttpContext.Session.GetString("token");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }

        public IActionResult Index()
        {
            var result = client.GetStringAsync(uri).Result;
            var list = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                AddHeader();
                var result = client.PostAsJsonAsync(uri, product).Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //chua dang nhap
                    return Redirect("/Account/Index");
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    //sai role
                    TempData["error"] = "Vui long login role admin de thuc hien!";
                    return Redirect("/Account/Index");
                }
            }
            return View();
        }
    }
}
