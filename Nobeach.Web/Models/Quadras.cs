using System.ComponentModel.DataAnnotations;    
using System.ComponentModel.DataAnnotations.Schema;

namespace Nobeach.Models
{
    public class Quadra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Esporte { get; set; } = string.Empty;
        public decimal ValorHora { get; set; }
        public string? Status { get; set; } = string.Empty;
    }
}