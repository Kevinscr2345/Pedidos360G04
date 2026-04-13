using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos360Grupo4.Data;
using Pedidos360Grupo4.Models;

namespace Pedidos360Grupo4.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly DatabaseLogic _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(DatabaseLogic context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            var clientes = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                clientes = clientes.Where(c => c.Nombre.Contains(searchString) || c.Cedula.Contains(searchString));
            }

            int tamanoPagina = 5;
            int numeroPagina = pageNumber ?? 1;
            int total = await clientes.CountAsync();

            var lista = await clientes
                .OrderBy(c => c.Nombre)
                .Skip((numeroPagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            ViewBag.CurrentFilter = searchString;
            ViewBag.PageIndex = numeroPagina;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)tamanoPagina);

            return View(lista);
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            return cliente == null ? NotFound() : View(cliente);
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Cliente registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al crear cliente.");
                ModelState.AddModelError("", "No se pudo guardar el cliente. Verifica que la cédula o el correo no estén repetidos.");
            }

            return View(cliente);
        }

        [Authorize(Roles = "Admin,Vendedor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FindAsync(id);
            return cliente == null ? NotFound() : View(cliente);
        }

        [Authorize(Roles = "Admin,Vendedor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Cliente actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al editar cliente.");
                ModelState.AddModelError("", "No se pudo actualizar el cliente.");
            }

            return View(cliente);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            return cliente == null ? NotFound() : View(cliente);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente != null)
                {
                    _context.Clientes.Remove(cliente);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Cliente eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente.");
                TempData["Error"] = "No se pudo eliminar el cliente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}