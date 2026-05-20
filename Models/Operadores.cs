using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace inter.Models
{
    [Table("operadores")]
    public class Operadores
    {
        [Key]
        [Column("op_codigo")]
        public int Id { get; set; }

    }
}