namespace Pedidos360Grupo4.ViewModels
{
    public class SolicitudCalculoPedido
    {
        public List<SolicitudCalculoPedidoLinea> Lineas { get; set; } = new();
    }

    public class SolicitudCalculoPedidoLinea
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; }
    }
}