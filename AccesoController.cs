using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pedidos360Grupo4.ViewModels;

namespace Pedidos360Grupo4.Controllers
{
    public class AccesoController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccesoController> _logger;

        public AccesoController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILogger<AccesoController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult IniciarSesion(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(LoginVM modelo, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _userManager.FindByEmailAsync(modelo.Correo);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
                return View(modelo);
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                usuario.UserName!,
                modelo.Contrasena,
                modelo.Recordarme,
                lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("Inicio de sesión correcto para {Correo}", modelo.Correo);

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
            return View(modelo);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("El usuario cerró sesión.");
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}