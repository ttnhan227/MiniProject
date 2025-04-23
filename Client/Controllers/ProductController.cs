﻿﻿﻿﻿﻿﻿﻿using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [Authorize(Roles = "admin")] // Add this attribute to restrict entire controller

    public class ProductController : Controller
    {
        private string uri = "https://localhost:7283/api/Product";
        private HttpClient client = new HttpClient();

        [NonAction]
        private void AddHeader()
        {
            if (HttpContext.Request.Cookies.TryGetValue("token", out string token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
        }
        [HttpGet]
        public IActionResult Index()
        {
            var result = client.GetStringAsync(uri).Result;
            var list = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);
            return View("Index", list);
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var result = client.GetStringAsync(uri + "/" + id).Result;
            var product = JsonConvert.DeserializeObject<Product>(result);
            return View(product);
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
            else
            {
            }

            return View();
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                AddHeader();
                var result = client.PutAsJsonAsync(uri + "/" + product.Id, product).Result;
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
            else
            {
            }

            return View();
        }

        public IActionResult Delete(int id)
        {
            AddHeader();
            var result = client.DeleteAsync(uri + "/" + id).Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
