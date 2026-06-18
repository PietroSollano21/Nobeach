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


namespace Barbearia.Controllers
{
   


    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }
        [HttpPost]
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
            
            return RedirectToAction("Login", "Usuario");
            }
    
        
        
       
    


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
       [HttpPost]
 public async Task<IActionResult> Login(string email, string senha)
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
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
               
               if(usuario.IsAdmin)
                {
                    return RedirectToAction("Admin", "Adm");
                }
                else{
                return RedirectToAction("Privacy", "Home");
                }
            }
            else
            {
            ViewBag.Error = "Email ou senha inválidos.";
            return View();
            }
        }
        [Authorize]
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