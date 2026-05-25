using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inter.Models
{
    [Table("pessoas")]
    public class Pessoas
    {
        [Key]
        [Column("codigo")]
        public int Id { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("cpf")]
        public string Cpf { get; set; }

        [Column("telefone")]
        public string? Telefone { get; set; }

        [Column("endereco")]
        public string Endereco { get; set; }

        [Column("numero")]
        public string Numero { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("senha")]
        public string Senha { get; set; }

        [Column("tipo")]
        public int Tipo { get; set; }
    }
}

