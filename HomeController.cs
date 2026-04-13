using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pedidos360Grupo4.Models;

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
            _logger.LogInformation("Se ingresó a la página de inicio.");
            return View();
        }

        public IActionResult Privacy()
        {
            _logger.LogInformation("Se ingresó a la vista Acerca del sistema.");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Administracion()
        {
            _logger.LogInformation("Un administrador ingresó al panel de administración.");
            return View();
        }

        public IActionResult CodigoEstado(int code)
        {
            ViewBag.Codigo = code;

            if (code == 404)
            {
                ViewBag.Mensaje = "La página que intentaste abrir no existe.";
                _logger.LogWarning("Se produjo un error 404.");
            }
            else if (code == 403)
            {
                ViewBag.Mensaje = "No tienes permiso para acceder a esta opción.";
                _logger.LogWarning("Se produjo un error 403.");
            }
            else if (code == 500)
            {
                ViewBag.Mensaje = "Se produjo un error interno en el servidor.";
                _logger.LogError("Se produjo un error 500.");
            }
            else
            {
                ViewBag.Mensaje = "Ocurrió un problema al procesar la solicitud.";
                _logger.LogWarning("Se produjo un código de estado no controlado: {Codigo}", code);
            }

            return View();
        }

        public IActionResult ProbarError500()
        {
            _logger.LogWarning("Se va a provocar un error 500 de prueba.");
            throw new Exception("Error 500 de prueba generado manualmente.");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            _logger.LogError("Se produjo un error no controlado. RequestId: {RequestId}", requestId);

            return View(new ErrorViewModel
            {
                RequestId = requestId
            });
        }
    }
}