using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos360Grupo4.Data;

namespace Pedidos360Grupo4.Controllers
{
    [Authorize(Roles = "Admin,Vendedor,Operaciones")]
    [ApiController]
    [Route("api/productos")]
    public class ApiProductosController : ControllerBase
    {
        private readonly DatabaseLogic _context;

        public ApiProductosController(DatabaseLogic context)
        {
            _context = context;
        }

        // Se cambió el parámetro a 'q' para cumplir con la ruta /api/productos/buscar?q=...
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar(string q = "")
        {
            var consulta = _context.Productos
                .Where(p => p.Activo && p.Stock > 0);

            if (!string.IsNullOrWhiteSpace(q))
            {
                // Búsqueda por nombre o código (Id) según la rúbrica
                consulta = consulta.Where(p => p.Nombre.Contains(q) || p.Id.ToString() == q);
            }

            var productos = await consulta
                .OrderBy(p => p.Nombre)
                .Take(10) 
                .Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    precio = p.Precio,
                    impuesto = p.ImpuestoPorc,
                    stock = p.Stock
                })
                .ToListAsync();

            return Ok(productos);
        }
    }
}