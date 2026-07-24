using Nobeach.Models;
using Nobeach.Services.Interfaces;
using Resend;
namespace Nobeach.Services;

public class EmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly IConfiguration _configuration;
public EmailService(IResend resend, IConfiguration configuration)
    {
        _resend = resend;
        _configuration = configuration;
    }
    public async Task EnviarConfirmacao(Usuario usuario)
    {
        var link = $"https://nobeach.com.br/Usuario/ConfirmarEmail?token={usuario.TokenConfirmacaoEmail}";
        var message = new EmailMessage();

        message.From = "Nobeach <noreply@nobeach.com.br>";
        message.To.Add(usuario.Email);

        message.Subject = "Confirme seu e-mail";

        message.HtmlBody = $@"
            <h2>Bem-vindo ao Nobeach!</h2>

            <p>Olá <strong>{usuario.Nome}</strong>.</p>

            <p>Obrigado por realizar seu cadastro.</p>

            <p>Clique no link abaixo para confirmar seu e-mail:</p>

            <p>
                <a href='{link}'>
                    Confirmar meu e-mail
                </a>
            </p>

            <br>

            <small>Se você não criou esta conta, ignore este e-mail.</small>
        ";

        await _resend.EmailSendAsync(message);
    }
    public async Task EnviarTrocaEmail(Usuario usuario)
    {
       var link = $"https://nobeach.com.br/Usuario/ConfirmarTrocaEmail?token={usuario.TokenTrocaEmail}";
        var message = new EmailMessage();
        message.From = "Nobeach <noreply@nobeach.com.br>";
        message.To.Add(usuario.NovoEmail);

        message.Subject = "Confirme a alteração do seu e-mail";

        message.HtmlBody = $@"
            <h2>Bem-vindo ao Nobeach!</h2>

            <p>Olá <strong>{usuario.Nome}</strong>.</p>

            <p>Recebemos uma solicitação de troca de Email.</p>

            <p>Clique no link abaixo para trocar seu e-mail:</p>

            <p>
                <a href='{link}'>
                    Trocar meu e-mail
                </a>
            </p>

            <br>

            <small>Se você não fez esta solicitação, ignore este e-mail.</small>
        ";

        await _resend.EmailSendAsync(message);
    }
    public async Task EnviarRecuperacaoSenha(Usuario usuario)
{
    var link = $"https://nobeach.com.br/Usuario/RedefinirSenha?token={usuario.TokenRecuperacaoSenha}";

    var message = new EmailMessage();

    message.From = "Nobeach <noreply@nobeach.com.br>";

    message.To.Add(usuario.Email);

    message.Subject = "Recuperação de senha";

    message.HtmlBody = $@"
        <h2>Recuperação de senha</h2>

        <p>Olá <strong>{usuario.Nome}</strong>.</p>

        <p>Recebemos uma solicitação para redefinir sua senha.</p>

        <p>Se foi você, clique no botão abaixo:</p>

        <p>
            <a href='{link}'>
                Redefinir minha senha
            </a>
        </p>

        <br>

        <small>
            Se você não solicitou esta alteração,
            ignore este e-mail.
        </small>
    ";

    await _resend.EmailSendAsync(message);
}
    }
