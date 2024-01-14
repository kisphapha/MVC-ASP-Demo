using Microsoft.AspNetCore.Mvc;
using MVCDemo.Models;
using System.Diagnostics;

namespace MVCDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            return View();
        }
        
        public IActionResult Privacy()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            return View();
        }
        public IActionResult Login()
        {
            ViewBag.user = Models.User.parseUser(HttpContext.Session.GetString("User"));
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}