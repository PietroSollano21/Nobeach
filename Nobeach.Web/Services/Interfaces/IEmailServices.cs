using Nobeach.Models;

namespace Nobeach.Services.Interfaces;

public interface IEmailService
{
    Task EnviarConfirmacao(Usuario usuario);
    Task EnviarTrocaEmail(Usuario usuario);
    Task EnviarRecuperacaoSenha(Usuario usuario);
}