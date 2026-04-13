using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Pedidos360Grupo4.ViewModels
{
    public class PedidoCrearVM
    {
        [Required(ErrorMessage = "Debes seleccionar un cliente.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        public List<PedidoLineaVM> Lineas { get; set; } = new();

        public List<SelectListItem> Clientes { get; set; } = new();
    }
}