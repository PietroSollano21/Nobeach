using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

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
    public bool IsAdmin => Perfil == "Admin";
    public bool EmailConfirmado { get; set;} = false;
    public string? TokenConfirmacaoEmail { get; set;}
    public DateTime? ExpiracaoConfirmacaoEmail { get; set; }
    public DateTime? DataConfirmacaoEmail { get; set; }
    public string? TokenTrocaEmail {get; set;}
    public DateTime? ExpiracaoTrocaEmail {get; set;}
    public string? NovoEmail {get; set;}
    public string? TokenRecuperacaoSenha { get; set;}
    public DateTime? ExpiracaoRecuperacaoSenha {get; set;}
}
}