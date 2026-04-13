using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos360Grupo4.Data;
using Pedidos360Grupo4.ViewModels;

namespace Pedidos360Grupo4.Controllers
{
    [Authorize(Roles = "Admin,Vendedor")]
    [ApiController]
    [Route("api/pedidos")]
    public class ApiPedidosController : ControllerBase
    {
        private readonly DatabaseLogic _context;

        public ApiPedidosController(DatabaseLogic context)
        {
            _context = context;
        }

        [HttpPost("calcular")]
        public async Task<IActionResult> Calcular([FromBody] SolicitudCalculoPedido solicitud)
        {
            if (solicitud == null || solicitud.Lineas == null || !solicitud.Lineas.Any())
            {
                return Ok(new RespuestaCalculoPedido());
            }

            var ids = solicitud.Lineas.Select(x => x.ProductoId).Distinct().ToList();

            var productos = await _context.Productos
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            decimal subtotal = 0;
            decimal impuestos = 0;
            decimal total = 0;

            var respuesta = new RespuestaCalculoPedido();

            foreach (var linea in solicitud.Lineas)
            {
                if (!productos.ContainsKey(linea.ProductoId))
                    continue;

                var producto = productos[linea.ProductoId];

                decimal precioBase = producto.Precio * linea.Cantidad;
                decimal montoDescuento = precioBase * (linea.Descuento / 100m);
                decimal baseConDescuento = precioBase - montoDescuento;
                decimal impuesto = baseConDescuento * (producto.ImpuestoPorc / 100m);
                decimal totalLinea = baseConDescuento + impuesto;

                subtotal += baseConDescuento;
                impuestos += impuesto;
                total += totalLinea;

                respuesta.Lineas.Add(new RespuestaCalculoPedidoLinea
                {
                    ProductoId = producto.Id,
                    PrecioUnitario = Math.Round(producto.Precio, 2),
                    ImpuestoPorcentaje = Math.Round(producto.ImpuestoPorc, 2),
                    TotalLinea = Math.Round(totalLinea, 2)
                });
            }

            respuesta.Subtotal = Math.Round(subtotal, 2);
            respuesta.Impuestos = Math.Round(impuestos, 2);
            respuesta.Total = Math.Round(total, 2);

            return Ok(respuesta);
        }
    }
}