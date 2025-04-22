using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using Newtonsoft.Json;

namespace Client.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly string uri = "https://localhost:7283/api/Home"; //
    private HttpClient client = new HttpClient();//
    
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        this.client = new HttpClient();
    }

    public IActionResult Index()
    {
        return View();

        //var result = client.GetStringAsync(uri).Result;
        //var list = JsonConvert.DeserializeObject<IEnumerable<HomeController>>(result);
        //return View(list);
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