using Microsoft.AspNetCore.Mvc;
using Nobeach.Data;
using Nobeach.Models;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Nobeach.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nobeach.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using AspNetCoreGeneratedDocument;


namespace Nobeach.Controllers
{
    // Controller responsável por operações relacionadas a usuários:
    // - Cadastro de novos usuários
    // - Autenticação (login / logout)
    // - Redirecionamentos pós-login conforme perfil

    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        // Injeção do contexto de dados (EF Core)
        public UsuarioController(AppDbContext context,IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        [HttpGet]
        // Formulário de cadastro (GET)
        public IActionResult Cadastro()
        {
            return View();
        }
        [HttpPost]
        // Recebe o POST do formulário de cadastro, cria usuário e autentica
        public async Task<IActionResult> Cadastro([Bind("Id,Nome,Email,Senha")] Usuario usuario)
        {
            var usuarioExistente = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email || u.Nome == usuario.Nome); ;
            if (usuarioExistente != null)
            {
                ViewBag.Erro = "Email ou nome já cadastrados.";
                return View();
            }
            
           
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            _context.Usuarios.Add(usuario);
            
            usuario.TokenConfirmacaoEmail = Guid.NewGuid().ToString();
            usuario.ExpiracaoConfirmacaoEmail = DateTime.UtcNow.AddHours(24);
            try
            {
            await _emailService.EnviarConfirmacao(usuario);
            }
            catch
            {
                TempData["Aviso"] = "Não foi possível enviar um novo email, solicite um novo envio";
            }
            _context.SaveChanges();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("Id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Perfil ?? "Cliente")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            TempData["Sucesso"] ="Cadastro realizado! Enviamos um email de confirmaçao para sua conta de email";
            return RedirectToAction("Index", "Home");
            }
        [HttpGet]
        // Página de login (GET). Também usada quando o modal não é empregado.
        public IActionResult Login(string returnUrl = "/Home/Privacy")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        // Logout do usuário — limpa o cookie de autenticação
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Email()
        {
            return View();
        }
        [HttpPost]
        // Autenticação: valida email/senha, cria claims e redireciona
        // Se o usuário for admin, envia para /Adm/Admin; senão para returnUrl ou /Home/Privacy
        public async Task<IActionResult> Login(string email, string senha, string returnUrl)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario != null && !string.IsNullOrEmpty(usuario.SenhaHash) && BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            {
                 
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Email),
                    new Claim("Id", usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Perfil ?? "Cliente")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                IsPersistent = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
               
               if(usuario.IsAdmin)
                {
                    return RedirectToAction("Admin", "Adm");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Privacy", "Home");
                }
            }
            ViewBag.Error = "Email ou senha inválidos.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [Authorize]
        // Exemplo de rota protegida que encaminha administradores ao painel
        public IActionResult Dashboard()
        {
            if (User.Identity.IsAuthenticated || User.IsInRole("Admin"))
            {
                return RedirectToAction("Admin", "Adm");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmarEmail(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                TempData["Erro"] = "Link de confirmação inválido.";
                return View("Email");
            }
            var usuario = _context.Usuarios.FirstOrDefault( u=> u.TokenConfirmacaoEmail == token);
            if(usuario == null)
            {
                return View("Expirado");
            }
            if (usuario.ExpiracaoConfirmacaoEmail == null ||usuario.ExpiracaoConfirmacaoEmail < DateTime.UtcNow)
            {
                return View("Expirado");
            }

            usuario.EmailConfirmado = true;
            usuario.DataConfirmacaoEmail = DateTime.UtcNow;
            usuario.TokenConfirmacaoEmail = null;
            usuario.ExpiracaoConfirmacaoEmail = null;

            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Seu e-mail foi confirmado com sucesso";
            return View("Confirmado");
        }
        [HttpPost]
public async Task<IActionResult> ReenviarConfirmacao(string email)
{
    var usuario =  _context.Usuarios.FirstOrDefault(u => u.Email == email);
    Console.WriteLine($"EmailConfirmado = {usuario.EmailConfirmado}");
    if (usuario == null)
    {
        return View("Email");
    }

    if (usuario.EmailConfirmado)
    {
        TempData["Sucesso"] = "Seu e-mail já está confirmado.";
        return RedirectToAction("Privacy", "Home");
    }

    usuario.TokenConfirmacaoEmail = Guid.NewGuid().ToString();
    usuario.ExpiracaoConfirmacaoEmail = DateTime.UtcNow.AddHours(24);

    await _context.SaveChangesAsync();

    await _emailService.EnviarConfirmacao(usuario);

    TempData["Sucesso"] = "Enviamos um novo e-mail de confirmação.";

    return RedirectToAction("Email");
}
    
    [HttpGet]
    public IActionResult AlterarEmail()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            string emailLogado = User.Identity!.Name!;
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);
            ViewBag.EmailAtual = usuario?.Email;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AlterarEmail(string novoEmail)
        {
            if(!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            string emailLogado = User.Identity.Name!;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == emailLogado);
            novoEmail = novoEmail.Trim().ToLower();
            if(usuario == null)
            {
                return RedirectToAction("Login");
            }
            if (string.IsNullOrWhiteSpace(novoEmail))
            {
                ModelState.AddModelError("", "Informe um e-mail válido.");

                return View(usuario);
            }
            bool emailExiste = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == novoEmail && u.Id != usuario.Id);

            if (emailExiste)
            {
                ModelState.AddModelError("", "Este e-mail já está cadastrado.");
                return View(usuario);
            }
            if (usuario.Email == novoEmail)
            {
                ModelState.AddModelError("", "Informe um e-mail diferente do atual.");

                return View(usuario);
            }
            usuario.NovoEmail = novoEmail;
            usuario.TokenTrocaEmail = Guid.NewGuid().ToString();
            usuario.ExpiracaoTrocaEmail = DateTime.UtcNow.AddHours(24);
            await _context.SaveChangesAsync();
            await _emailService.EnviarTrocaEmail(usuario);
            return View("AlterarEmail");
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmarTrocaEmail(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                return View("Expirado");
            }
            Console.WriteLine("1 - Entrou na Action");
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenTrocaEmail == token);
            Console.WriteLine("2 - Procurou usuário");
            if(usuario == null)
            {
               Console.WriteLine("3 - Usuário não encontrado");
               return View("Expirado") ;
            }
            if(string.IsNullOrWhiteSpace(usuario.NovoEmail))
            {
                return View("TokenInvalido");
            }
            Console.WriteLine("4 - Usuário encontrado");
            if(usuario.ExpiracaoTrocaEmail == null || usuario.ExpiracaoTrocaEmail < DateTime.UtcNow)
            {
                Console.WriteLine("5 - Token expirado");
                return View("Expirado");
            }
            bool emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == usuario.NovoEmail && u.Id != usuario.Id);
            if(emailExiste)
            {
                TempData["Erro"] = "Este e-mail já está sendo utilizado por outra conta.";
                return RedirectToAction("AlterarEmail");
            }
            Console.WriteLine("6 - Token válido");
            usuario.Email = usuario.NovoEmail!;
            if (!usuario.EmailConfirmado)
            {
                usuario.EmailConfirmado = true;
                usuario.DataConfirmacaoEmail = DateTime.UtcNow;
                usuario.TokenConfirmacaoEmail = null;
                usuario.ExpiracaoTrocaEmail = null;
            }
            usuario.NovoEmail = null;
            usuario.TokenTrocaEmail = null;
            usuario.ExpiracaoTrocaEmail = null;
            await _context.SaveChangesAsync();
            await HttpContext.SignOutAsync();

            TempData["Sucesso"] = "Seu e-mail foi alterado com sucesso.";

            return RedirectToAction("Login", "Usuario");
        }
        [HttpGet]
        public IActionResult EsqueciSenha()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>EsqueciSenha(string email)
        {
          email = email.Trim().ToLower();

    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

    // Sempre retorna a mesma mensagem
    if (usuario == null)
    {
        TempData["Sucesso"] =
            "Se existir uma conta com este e-mail, enviaremos um link para recuperação da senha.";

        return RedirectToAction(nameof(EsqueciSenha));
    }

    usuario.TokenRecuperacaoSenha = Guid.NewGuid().ToString();
    usuario.ExpiracaoRecuperacaoSenha = DateTime.UtcNow.AddHours(24);

    await _context.SaveChangesAsync();

    await _emailService.EnviarRecuperacaoSenha(usuario);

    TempData["Sucesso"] =
        "Se existir uma conta com este e-mail, enviaremos um link para recuperação da senha.";

    return RedirectToAction(nameof(EsqueciSenha));
        }
    [HttpGet]
    public async Task<IActionResult> RedefinirSenha(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                return View("Expirado");
            }
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenRecuperacaoSenha == token);
            if (usuario == null)
            {
                return View("Expirado");
            }
            if(usuario.ExpiracaoRecuperacaoSenha == null || usuario.ExpiracaoRecuperacaoSenha < DateTime.UtcNow)
            {
                return View("Expirado");
            }
            ViewBag.Token = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RedefinirSenha(string token, string novaSenha, string confirmarSenha)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.TokenRecuperacaoSenha == token);
            if(usuario == null)
            {
                return View("Expirado");     
            }
            if (usuario.ExpiracaoRecuperacaoSenha == null ||
        usuario.ExpiracaoRecuperacaoSenha < DateTime.UtcNow)
    {
        return View("Expirado");
    }

    if (string.IsNullOrWhiteSpace(novaSenha))
    {
        ModelState.AddModelError("", "Informe uma senha.");

        ViewBag.Token = token;

        return View();
    }

    if (novaSenha != confirmarSenha)
    {
        ModelState.AddModelError("", "As senhas não coincidem.");

        ViewBag.Token = token;

        return View();
    }

    usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);

    usuario.TokenRecuperacaoSenha = null;
    usuario.ExpiracaoRecuperacaoSenha = null;

    await _context.SaveChangesAsync();

    TempData["Sucesso"] =
        "Sua senha foi alterada com sucesso.";

    return RedirectToAction("Login");
        }

    }
}
