using System.ComponentModel.DataAnnotations;

namespace Pedidos360Grupo4.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(9, ErrorMessage = "La cédula no puede superar mas de 9 numeros.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = null!;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
        [StringLength(200, ErrorMessage = "El correo no puede superar los 200 caracteres.")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "El Telefono es obligatorio.")]
        [StringLength(8, ErrorMessage = "El teléfono no puede superar los 8 numros.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La direccion es obligatorio.")]
        [StringLength(300, ErrorMessage = "La dirección no puede superar los 300 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}