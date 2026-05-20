using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inter.Models
{

    [Table("produtos")]
    public class Produtos
    {
        [Key]
        [Column("codigo_produto")]
        public int Id { get; set; }

        [Column("nome_produto")]
        public string Nome { get; set; }

        [Column("preco")]
        public decimal Preco { get; set; }

        [Column("quantidade")]
        public int Qtd { get; set; }

        [Column("id_tipo")]
        public int TipoId { get; set; }

        [Column("imagem")]
        public string? Imagem { get; set; }
        
        [NotMapped]
        public string NomeTipo { get; set; }

    }


}
