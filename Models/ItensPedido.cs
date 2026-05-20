using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inter.Models
{
    [Table("itens_pedido")]
    public class ItensPedido
    {
        [Key]
        [Column("codigo_item")]
        public int Id { get; set; }

        [Column("codigo_pedido")]
        public int PedidoId { get; set; }

        [Column("codigo_produto")]
        public int ProdutoId { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }

        // RELACIONAMENTOS
        [ForeignKey("PedidoId")]
        public Pedidos? Pedido { get; set; }

        [ForeignKey("ProdutoId")]
        public Produtos? Produto { get; set; }
    }
}