
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Nobeach.Models
{
    
    public class Diaquadra
    {
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }
        public string QuadraId { get; set; }
        [Key]
        public int Id { get; set; }
       public bool Disponivel { get; set;}
    }
    }