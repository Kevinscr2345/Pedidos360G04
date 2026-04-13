using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pedidos360Grupo4.Data;
using Pedidos360Grupo4.Models;
using Pedidos360Grupo4.ViewModels;

namespace Pedidos360Grupo4.Controllers
{
    [Authorize(Roles = "Admin,Vendedor")]
    public class PedidosController : Controller
    {
        private readonly DatabaseLogic _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(
            DatabaseLogic context,
            UserManager<IdentityUser> userManager,
            ILogger<PedidosController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string Buscar, string estado)
        {
            ViewBag.CurrentFilter = Buscar;
            ViewBag.CurrentEstado = estado;

            var pedidos = _context.Pedidos.Include(p => p.Cliente).AsQueryable();

            if (!string.IsNullOrEmpty(Buscar))
            {
                pedidos = pedidos.Where(p => p.Cliente.Nombre.Contains(Buscar) ||
                                             p.Id.ToString().Contains(Buscar));
            }

            if (!string.IsNullOrEmpty(estado))
            {
                pedidos = pedidos.Where(p => p.Estado == estado);
            }

            var estados = await _context.Pedidos.Select(p => p.Estado).Distinct().ToListAsync();

            ViewData["Estados"] = new SelectList(estados, estado);

            pedidos = pedidos.OrderByDescending(p => p.Fecha);

            return View(await pedidos.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
                return NotFound();

            return View(pedido);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new PedidoCrearVM
            {
                Clientes = await _context.Clientes
                    .OrderBy(c => c.Nombre)
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Nombre + " - " + c.Cedula
                    })
                    .ToListAsync()
            };

            ViewBag.Productos = await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToListAsync();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PedidoCrearVM vm)
        {
            vm.Clientes = await _context.Clientes
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre + " - " + c.Cedula
                })
                .ToListAsync();

            ViewBag.Productos = await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nombre
                })
                .ToListAsync();

            if (vm.Lineas == null || !vm.Lineas.Any())
            {
                ModelState.AddModelError("", "Debes agregar al menos un producto al pedido.");
                return View(vm);
            }

            if (!ModelState.IsValid)
                return View(vm);

            var productoIds = vm.Lineas.Select(x => x.ProductoId).Distinct().ToList();

            var productos = await _context.Productos
                .Where(p => productoIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var linea in vm.Lineas)
            {
                if (!productos.ContainsKey(linea.ProductoId))
                {
                    ModelState.AddModelError("", $"El producto con ID {linea.ProductoId} no existe.");
                    return View(vm);
                }

                if (linea.Cantidad <= 0)
                {
                    ModelState.AddModelError("", "Todas las cantidades deben ser mayores a cero.");
                    return View(vm);
                }

                if (productos[linea.ProductoId].Stock < linea.Cantidad)
                {
                    ModelState.AddModelError("", $"No hay stock suficiente para el producto: {productos[linea.ProductoId].Nombre}");
                    return View(vm);
                }
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return Challenge();

            decimal subtotal = 0;
            decimal impuestos = 0;
            decimal total = 0;

            var detalles = new List<PedidoDetalle>();

            foreach (var linea in vm.Lineas)
            {
                var producto = productos[linea.ProductoId];

                decimal precioBase = producto.Precio * linea.Cantidad;
                decimal montoDescuento = precioBase * (linea.Descuento / 100m);
                decimal baseConDescuento = precioBase - montoDescuento;
                decimal impuesto = baseConDescuento * (producto.ImpuestoPorc / 100m);
                decimal totalLinea = baseConDescuento + impuesto;

                subtotal += baseConDescuento;
                impuestos += impuesto;
                total += totalLinea;

                detalles.Add(new PedidoDetalle
                {
                    ProductoId = producto.Id,
                    Cantidad = linea.Cantidad,
                    PrecioUnit = producto.Precio,
                    Descuento = linea.Descuento,
                    ImpuestoPorc = producto.ImpuestoPorc,
                    TotalLinea = Math.Round(totalLinea, 2)
                });

                producto.Stock -= linea.Cantidad;
            }

            var pedido = new Pedido
            {
                ClienteId = vm.ClienteId,
                UsuarioId = usuario.Id,
                Fecha = DateTime.Now,
                Subtotal = Math.Round(subtotal, 2),
                Impuestos = Math.Round(impuestos, 2),
                Total = Math.Round(total, 2),
                Estado = "Registrado",
                Detalles = detalles
            };

            try
            {
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                TempData["Exito"] = "Pedido registrado correctamente.";
                _logger.LogInformation("Pedido {PedidoId} creado por {Usuario}", pedido.Id, usuario.Email);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar pedido.");
                ModelState.AddModelError("", "Ocurrió un error al guardar el pedido.");
                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null)
                return NotFound();

            pedido.Estado = nuevoEstado;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }
    }
}