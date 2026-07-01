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


namespace Nobeach.Controllers
{
    // Controller responsável por operações relacionadas a usuários:
    // - Cadastro de novos usuários
    // - Autenticação (login / logout)
    // - Redirecionamentos pós-login conforme perfil

    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        // Injeção do contexto de dados (EF Core)
        public UsuarioController(AppDbContext context)
        {
            _context = context;
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
                    new Claim(ClaimTypes.Role, usuario.Perfil)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
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
            else
            {
                ViewBag.Error = "Email ou senha inválidos.";
                //ViewBag.ReturnUrl = returnUrl;
                return View();
            }
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
}
}