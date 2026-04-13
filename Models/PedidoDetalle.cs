using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos360Grupo4.Models
{
    public class PedidoDetalle
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnit { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal ImpuestoPorc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalLinea { get; set; }

        public Pedido Pedido { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
    }
}