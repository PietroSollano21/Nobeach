using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nobeach.Models
{
public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; }= string.Empty;

    [NotMapped]
    public string Senha { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string? Perfil { get; set; } = "Cliente";
    public bool IsAdmin => Perfil == "Barbeiro" || Perfil == "Admin";
    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "O CPF deve estar no formato XXX.XXX.XXX-XX.")]
    public string? CPF {get; set;} = string.Empty;
}
}