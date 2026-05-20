using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inter.Models
{
    [Table("clientes")]
    public class Clientes
    {
        [Key]
        [Column("codigo_cliente")]
        public int Id { get; set; }


    }

}