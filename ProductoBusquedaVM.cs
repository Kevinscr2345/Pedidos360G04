namespace Pedidos360Grupo4.ViewModels
{
    public class ProductoBusquedaVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal ImpuestoPorc { get; set; }
        public int Stock { get; set; }
    }
}