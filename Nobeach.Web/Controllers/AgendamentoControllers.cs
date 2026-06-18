using Microsoft.AspNetCore.Mvc;
using Nobeach.Models; 
using Nobeach.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Reflection.PortableExecutable;
using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using Nobeach.Data;
using Microsoft.EntityFrameworkCore;

namespace Barbearia.Controllers
{
    
    public class AgendamentoController : Controller
    {
    
        private readonly AgendamentoRepositories _agendamentoRepository;
        private readonly AppDbContext _context;
        private readonly AgendamentoRepositories _repo;
        private readonly IConfiguration _configuration;
       
        public  AgendamentoController(AgendamentoRepositories agendamentoRepository, IConfiguration configuration, AppDbContext context)
        {
            _agendamentoRepository = agendamentoRepository;
            _configuration = configuration;
            _context= context;
        }

        [HttpPost]
        public IActionResult Criar(Agendamento agendamento)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _agendamentoRepository.Adicionar(agendamento);

            return Ok(agendamento);
        }

        [Authorize]
[HttpGet("~/Home/Agendar")]
public async Task<IActionResult> Agendar(DateTime? dataSelecionada)
{
    var barbeiroId = User.Identity.Name;
    DateTime data = dataSelecionada ?? DateTime.Today;
    var BarbeiroNome = await _context.Usuarios.Where(u => u.Perfil == "Barbeiro").Select(u => u.Nome).ToListAsync();
    ViewBag.BarbeiroNome = BarbeiroNome;
    List<TimeSpan> grandeTotal = new List<TimeSpan>
    {
        new TimeSpan(9, 0, 0),  new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0),
        new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0),
        new TimeSpan(17, 0, 0), new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0)
    };
    var repo = new AgendamentoRepositories(_context, _configuration);
   var ocupados = repo.BuscarHorariosOcupados(data);
    var disponiveis = grandeTotal.Where(h => !ocupados.Contains(h)).ToList();
    ViewBag.HorariosDisponiveis = disponiveis.Select(h => h.ToString(@"hh\:mm")).ToList();
    ViewBag.DataSelecionada = data.ToString("yyyy-MM-dd");
//var datasBloqueadas = await _context.Status.Where(d => d.BarbeiroId == barbeiroId && !d.Disponivel && d.Data >= DateTime.Today).Select(d => d.Data.ToString("yyyy-MM-dd")).ToListAsync();
    //ViewBag.DatasBloqueadas= datasBloqueadas;
    return View();
}

[Authorize]
[HttpPost("~/Home/Agendar")]
public async Task<IActionResult> Agendar(Agendamento model)
{
    var barbeiroId = User.Identity.Name;
    var emailBarbeiro = await _context.Usuarios
    .Where(u => u.Nome == model.BarbeiroNome)
.Select(u => u.Email)
    .FirstOrDefaultAsync();
var datasBloqueadas = await _context.DiaBarbeiros
    .Where(d => !d.Disponivel &&
                d.Data >= DateTime.Today)
    .Select(d => d.Data.ToString("yyyy-MM-dd"))
    .ToListAsync();
    ViewBag.DatasBloqueadas = datasBloqueadas;
    Console.WriteLine($"BarbeiroId salvo no banco: {User.Identity?.Name}");
Console.WriteLine($"BarbeiroNome do agendamento: {model.BarbeiroNome}");
 var bloqueado = await _context.DiaBarbeiros.AnyAsync(d => d.BarbeiroId == emailBarbeiro && d.Data.Date == model.Data.Date && !d.Disponivel);
    Console.WriteLine($"Email {emailBarbeiro} encontrado para o barbeiro.");
    Console.WriteLine($"Data do agendamento: {model.Data.Date}");
    Console.WriteLine($"Quadra bloqueada nesta data? {bloqueado}");
    if(bloqueado)
            {
              ModelState.AddModelError("", "A quadra está indisponível nesta data.");
              return View("Agenda", model);  
            }
    if (ModelState.IsValid)
    {
        var _repo = new AgendamentoRepositories(_context, _configuration);
        _repo.SalvarAgendamento(model);
        return RedirectToAction("Dashboard", "Home");
    }
    DateTime data = model.Data != DateTime.MinValue ? model.Data : DateTime.Today;
    ViewBag.Quadras = Quadras;
    List<TimeSpan> grandeTotal = new List<TimeSpan>
    {
        new TimeSpan(9, 0, 0),  new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0),
        new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0),
        new TimeSpan(17, 0, 0), new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0)
    };
    var repo = new AgendamentoRepository(_context, _configuration);
    var ocupados = repo.BuscarHorariosOcupados(data);
    var disponiveis = grandeTotal.Where(h => !ocupados.Contains(h)).ToList();
    ViewBag.HorariosDisponiveis = disponiveis.Select(h => h.ToString(@"hh\:mm")).ToList();
    ViewBag.DataSelecionada = data.ToString("yyyy-MM-dd");
    ViewBag.DatasBloqueadas = datasBloqueadas;
     ModelState.AddModelError("", "O horário selecionado já está ocupado. Por favor, escolha outro horário.");
     return View("Agenda", model);
}
        [HttpGet("Agendamento/BuscarBarbeiros")]
        public async Task<IActionResult> BuscarBarbeiros()
        {
            var Quadras = await _context.Usuarios.Where(u => u.Perfil == "Barbeiro").Select(u => u.Nome).ToListAsync();
            return Json(Quadras);
        }
        
        
       
    }

}