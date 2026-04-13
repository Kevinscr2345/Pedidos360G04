using System.ComponentModel.DataAnnotations;

namespace Pedidos360Grupo4.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
