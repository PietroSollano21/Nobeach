using System.ComponentModel.DataAnnotations;

namespace Nobeach.Web.Models
{
    public class Diaquadra
    {
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }
        public string QuadraId { get; set; } = null!;
        [Key]
        public int Id { get; set; }
       public bool Disponivel { get; set;}
    }
    }