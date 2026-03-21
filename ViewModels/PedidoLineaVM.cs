using System.ComponentModel.DataAnnotations;

namespace Pedidos360Grupo4.ViewModels
{
    public class PedidoLineaVM
    {
        public int ProductoId { get; set; }

        public string ProductoNombre { get; set; } = string.Empty;

        [Range(1, 999, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Cantidad { get; set; }

        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
        public decimal Descuento { get; set; }
    }
}