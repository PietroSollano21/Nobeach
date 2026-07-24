using System.ComponentModel.DataAnnotations.Schema;
using Nobeach.Enums;

namespace Nobeach.Models
{
    [Table("agendamento")]
    public class Agendamento
    {
        public long? Id { get; set; }
        public string? NomeCliente { get; set; } = string.Empty;
        [Column("DataDia")]
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public Esporte Esporte { get; set; }
        //public decimal Valor { get; set; }
        //public required string? statuspagamento  { get; set; }
        //public long? PaymentId { get; set; }
        public bool Cancelado { get; set;}
        public string? Quadra { get; set; } = string.Empty; 
        public string EmailCliente { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        
        public string? Telefone { get; set; } = string.Empty;
    }
}