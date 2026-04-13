using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos360Grupo4.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;

        public DateTime Fecha { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Impuestos { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } = EstadosPedido.Registrado;

        public Cliente Cliente { get; set; } = null!;
        public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    }

    public static class EstadosPedido
    {
        public const string Registrado = "Registrado";
        public const string EnProceso = "En Proceso";
        public const string Completado = "Completado";
        public const string Cancelado = "Cancelado";
    }
}