using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inter.Models
{
    [Table("tipo_pessoas")]
    public class TipoPessoa
    {
        [Key]
        [Column("id_tipo_pessoa")]
        public int Id { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

    }
}