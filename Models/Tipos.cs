using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inter.Models
{
    [Table("tipos")]
    public class Tipos
    {
        [Key]
        [Column("id_tipo")]
        public int Id { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }


        [Column("obs")]
        public string Observacao { get; set; }

    }
}