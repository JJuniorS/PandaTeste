using System.ComponentModel.DataAnnotations;

namespace pandaTeste.api.Domain.Models
{
    public class Financeiro
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Descricao { get; set; }

        [Required]
        public decimal Valor { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoFinanceiro { get; set; } // "Entrada" ou "Saída"

        public bool Baixado { get; set; }

        [Required]
        public DateTime DtVencimento { get; set; }

        public DateTime? DtCadastro { get; set; }

        public DateTime? DtBaixa { get; set; }
    }
}
