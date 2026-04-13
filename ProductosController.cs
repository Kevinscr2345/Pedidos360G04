using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pedidos360Grupo4.Data;
using Pedidos360Grupo4.Models;

namespace Pedidos360Grupo4.Controllers
{
    [Authorize(Roles = "Admin,Vendedor")]
    public class ProductosController : Controller
    {
        private readonly DatabaseLogic _context;
        private readonly IWebHostEnvironment _entorno;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(DatabaseLogic context, IWebHostEnvironment entorno, ILogger<ProductosController> logger)
        {
            _context = context;
            _entorno = entorno;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchString, int? categoriaId, int? pageNumber)
        {
            var productos = _context.Productos.Include(p => p.Categoria).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                productos = productos.Where(p => p.Nombre.Contains(searchString));
            }

            if (categoriaId != null)
            {
                productos = productos.Where(p => p.CategoriaId == categoriaId);
            }

            int tamanoPagina = 5;
            int numeroPagina = pageNumber ?? 1;
            int totalRegistros = await productos.CountAsync();

            var listaPaginada = await productos
                .OrderBy(p => p.Nombre)
                .Skip((numeroPagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentCategory = categoriaId;
            ViewBag.PageIndex = numeroPagina;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRegistros / (double)tamanoPagina);

            ViewData["Categorias"] = new SelectList(_context.Categorias, "Id", "Nombre", categoriaId);

            return View(listaPaginada);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto, IFormFile? imagenFile)
        {
            ModelState.Remove("Categoria");

            try
            {
                if (ModelState.IsValid)
                {
                    if (imagenFile != null && imagenFile.Length > 0)
                    {
                        string carpeta = Path.Combine(_entorno.WebRootPath, "images");

                        if (!Directory.Exists(carpeta))
                        {
                            Directory.CreateDirectory(carpeta);
                        }

                        string nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await imagenFile.CopyToAsync(stream);
                        }

                        producto.ImagenUrl = "/images/" + nombreArchivo;
                    }

                    _context.Add(producto);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Producto creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto.");
                ModelState.AddModelError("", "No se pudo guardar el producto.");
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Producto producto, IFormFile? imagenFile)
        {
            if (id != producto.Id) return NotFound();

            ModelState.Remove("Categoria");

            try
            {
                if (ModelState.IsValid)
                {
                    if (imagenFile != null && imagenFile.Length > 0)
                    {
                        string carpeta = Path.Combine(_entorno.WebRootPath, "images");

                        if (!Directory.Exists(carpeta))
                        {
                            Directory.CreateDirectory(carpeta);
                        }

                        string nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await imagenFile.CopyToAsync(stream);
                        }

                        producto.ImagenUrl = "/images/" + nombreArchivo;
                    }
                    else
                    {
                        var productoAnterior = await _context.Productos
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.Id == id);

                        if (productoAnterior != null)
                        {
                            producto.ImagenUrl = productoAnterior.ImagenUrl;
                        }
                    }

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Producto actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar producto.");
                ModelState.AddModelError("", "No se pudo actualizar el producto.");
            }

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var producto = await _context.Productos.FindAsync(id);
                if (producto != null)
                {
                    _context.Productos.Remove(producto);
                    await _context.SaveChangesAsync();
                    TempData["Exito"] = "Producto eliminado correctamente.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto.");
                TempData["Error"] = "No se pudo eliminar el producto.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}