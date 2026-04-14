using Microsoft.AspNetCore.Mvc;
using Pedidos360Grupo4.Models;
using System.Diagnostics;

namespace Pedidos360Grupo4.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            // Capturamos el código de error (ej. 404 o 500) para mostrar un mensaje personalizado
            if (statusCode.HasValue)
            {
                ViewBag.StatusCode = statusCode.Value;
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}