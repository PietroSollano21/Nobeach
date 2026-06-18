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

namespace Nobeach.Controllers
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
[HttpGet("~/Home/Agenda")]
public async Task<IActionResult> Agenda(DateTime? dataSelecionada)
{
    
    DateTime data = dataSelecionada ?? DateTime.Today;
   
    List<TimeSpan> grandeTotal = new List<TimeSpan>
    {
        new TimeSpan(6,0 ,0),new TimeSpan(7, 0, 0),  new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0),
        new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0),
        new TimeSpan(17, 0, 0), new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0),
        new TimeSpan(20, 0, 0), new TimeSpan(21, 0, 0)
    };
    var repo = new AgendamentoRepositories(_context, _configuration);
   var ocupados = repo.BuscarHorariosOcupados(data);
    var disponiveis = grandeTotal.Where(h => !ocupados.Contains(h)).ToList();
    ViewBag.HorariosDisponiveis = disponiveis.Select(h => h.ToString(@"hh\:mm")).ToList();
    ViewBag.DataSelecionada = data.ToString("yyyy-MM-dd");
    var quadras = await _context.Quadras.ToListAsync();
ViewBag.Quadras = quadras;
//var datasBloqueadas = await _context.Status.Where(d => d.BarbeiroId == barbeiroId && !d.Disponivel && d.Data >= DateTime.Today).Select(d => d.Data.ToString("yyyy-MM-dd")).ToListAsync();
    //ViewBag.DatasBloqueadas= datasBloqueadas;
    return View("~/Views/Home/Agenda.cshtml");
}

[Authorize]
[HttpPost("~/Home/Agenda")]
public async Task<IActionResult> Agenda(Agendamento model)
{
var datasBloqueadas = await _context.Diaquadras
    .Where(d => !d.Disponivel &&
                d.Data >= DateTime.Today)
    .Select(d => d.Data.ToString("yyyy-MM-dd"))
    .ToListAsync();
    ViewBag.DatasBloqueadas = datasBloqueadas;
    Console.WriteLine($"BarbeiroId salvo no banco: {User.Identity?.Name}");

 var bloqueado = await _context.Diaquadras.AnyAsync(d => d.QuadraId == model.Quadra && d.Data.Date == model.Data.Date && !d.Disponivel);
    Console.WriteLine($"Data do agendamento: {model.Data.Date}");
    Console.WriteLine($"Quadra bloqueada nesta data? {bloqueado}");
    if(bloqueado)
            {
              ModelState.AddModelError("", "A quadra está indisponível nesta data.");
              return View("~/Views/Home/Agenda.cshtml", model);
            }
    if (ModelState.IsValid)
    {
        var _repo = new AgendamentoRepositories(_context, _configuration);
        _repo.SalvarAgendamento(model);
        return RedirectToAction("Privacy", "Home");
    }
    DateTime data = model.Data != DateTime.MinValue ? model.Data : DateTime.Today;
    var quadras = await _context.Quadras.ToListAsync();
    ViewBag.Quadras = quadras;
    List<TimeSpan> grandeTotal = new List<TimeSpan>
    {
        new TimeSpan(6,0 ,0),new TimeSpan(7, 0, 0),  new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0),
        new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0),
        new TimeSpan(17, 0, 0), new TimeSpan(18, 0, 0), new TimeSpan(19, 0, 0),
        new TimeSpan(20, 0, 0), new TimeSpan(21, 0, 0)
    };
    var repo = new AgendamentoRepositories(_context, _configuration);
    var ocupados = repo.BuscarHorariosOcupados(data);
    var disponiveis = grandeTotal.Where(h => !ocupados.Contains(h)).ToList();
    ViewBag.HorariosDisponiveis = disponiveis.Select(h => h.ToString(@"hh\:mm")).ToList();
    ViewBag.DataSelecionada = data.ToString("yyyy-MM-dd");
    ViewBag.DatasBloqueadas = datasBloqueadas;
     ModelState.AddModelError("", "O horário selecionado já está ocupado. Por favor, escolha outro horário.");
     return View("~/Views/Home/Agenda.cshtml", model);
}
        [HttpGet("Agendamento/BuscarQuadras")]
        public async Task<IActionResult> BuscarQuadras()
        {
            var Quadras = await _context.Quadras.ToListAsync();
            return Json(Quadras);
        }
        
        
       
    }

}