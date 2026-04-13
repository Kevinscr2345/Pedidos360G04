namespace Pedidos360Grupo4.ViewModels
{
    public class RespuestaCalculoPedido
    {
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public List<RespuestaCalculoPedidoLinea> Lineas { get; set; } = new();
    }

    public class RespuestaCalculoPedidoLinea
    {
        public int ProductoId { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal ImpuestoPorcentaje { get; set; }
        public decimal TotalLinea { get; set; }
    }
}