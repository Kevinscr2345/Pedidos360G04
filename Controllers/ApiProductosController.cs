using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos360Grupo4.Data;
using Pedidos360Grupo4.ViewModels;

namespace Pedidos360Grupo4.Controllers
{
    [Authorize(Roles = "Admin,Vendedor")]
    [ApiController]
    [Route("api/productos")]
    public class ApiProductosController : ControllerBase
    {
        private readonly DatabaseLogic _context;

        public ApiProductosController(DatabaseLogic context)
        {
            _context = context;
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar(string termino = "")
        {
            var consulta = _context.Productos
                .Where(p => p.Activo && p.Stock > 0);

            if (!string.IsNullOrWhiteSpace(termino))
            {
                consulta = consulta.Where(p => p.Nombre.Contains(termino));
            }

            var productos = await consulta
                .OrderBy(p => p.Nombre)
                .Take(20)
                .Select(p => new ProductoBusquedaVM
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    ImpuestoPorc = p.ImpuestoPorc,
                    Stock = p.Stock
                })
                .ToListAsync();

            return Ok(productos);
        }
    }
}