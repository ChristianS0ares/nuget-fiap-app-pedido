using System.ComponentModel.DataAnnotations;

namespace nuget_fiap_app_pedido_common.Models
{
    public class Item
    {
        [Required]
        public int Id { get; set; }
        public  string Descricao { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}
