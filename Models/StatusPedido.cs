using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inter.Models
{
    [Table("status_pedido")]
    public class StatusPedido
    {
        [Key]
        [Column("id_status")]
        public int Id { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }
    }
}