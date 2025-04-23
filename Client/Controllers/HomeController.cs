using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using Newtonsoft.Json;

namespace Client.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly string uri = "https://localhost:7283/api/Product"; //
    private HttpClient client = new HttpClient();//

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        this.client = new HttpClient();
    }

    public IActionResult Index()
    {
        //return View();
        try{
        var result = client.GetStringAsync(uri).Result;
        var list = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);
        return View(list);
        }
        catch(Exception ex){
            return View(new List<Product>());
        }
    }

public IActionResult Search(string search)
    {
        string url = "https://localhost:7283/api/Product/Search?searchTerm=" + Uri.EscapeDataString(search);
        var result = client.GetStringAsync(url).Result;
        var list = JsonConvert.DeserializeObject<IEnumerable<Product>>(result);
        return View("Index", list);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
