using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inter.Models
{
    [Table("pedidos")]
    public class Pedidos
    {
        [Key]
        [Column("codigo_pedidos")]
        public int Id { get; set; }

        [Column("codigo_cliente")]
        public int ClienteId { get; set; }

        [Column("data_pedido")]
        public DateTime DataPedido { get; set; }

        [Column("data_final")]
        public DateTime? DataFinal { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        // RELACIONAMENTO
        public List<ItensPedido>? Itens { get; set; }

        [Column("id_status")]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public StatusPedido? Status { get; set; }

        
    }
}