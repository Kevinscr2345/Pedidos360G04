using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos360Grupo4.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; } = null!;

        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El precio del producto es obligatorio.")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio")]
        [Range(0.01, 9999999, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El impuesto del producto es obligatorio.")]
        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Impuesto (%)")]
        [Range(0, 100, ErrorMessage = "El impuesto debe estar entre 0 y 100.")]
        public decimal ImpuestoPorc { get; set; }

        [Required(ErrorMessage = "El Stock del producto es obligatorio.")]
        [Display(Name = "Stock disponible")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [MaxLength(400)]
        [Display(Name = "Imagen")]
        public string? ImagenUrl { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public Categoria Categoria { get; set; } = null!;
    }
}