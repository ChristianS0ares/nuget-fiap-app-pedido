using System.ComponentModel.DataAnnotations;

namespace nuget_fiap_app_pedido_common.Models
{
    public class Pedido
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Cliente? Cliente { get; set; }
        [Required]
        public List<Item> Itens { get; set; }
        public DateTime Data { get; set; }
        public decimal Total { get; set; }
    }
}
