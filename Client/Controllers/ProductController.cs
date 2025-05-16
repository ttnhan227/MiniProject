using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [Authorize(Roles = "admin")] // Add this attribute to restrict entire controller

    public class ProductController : Controller
    {
        private string uri = "https://localhost:7288/api/Product/";
        private HttpClient client = new HttpClient();

        [NonAction]
        private void AddHeader()
        {
            if (HttpContext.Request.Cookies.TryGetValue("token", out string token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
        }

        //public IActionResult Index()
        //{
        //    var result = client.GetStringAsync(uri).Result;
        //    var list = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);
        //    return View(list);
        //}

        //public IActionResult Create()
        //{
        //    return View();
        //}

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

        [HttpGet]
        public IActionResult Index(string search)
        {
            AddHeader();
            string url = uri;
            if (!string.IsNullOrEmpty(search))
            {
                url = uri + "Search?searchTerm=" + search;
            }
            try
            {
                var result = client.GetStringAsync(url).Result;
                var list = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);
                return View(list);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("403"))
                {
                    TempData["error"] = "Access denied. Admin role required.";
                    return Redirect("/Account/Index");
                }
                throw;
            }
        }
    }
}
