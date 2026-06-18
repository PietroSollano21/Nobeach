using System.ComponentModel.DataAnnotations;

<<<<<<< HEAD
namespace Nobeach.Web.Models
=======
namespace Nobeach.Models
>>>>>>> 0397fc91991435870725c4404aa070c56684960c
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