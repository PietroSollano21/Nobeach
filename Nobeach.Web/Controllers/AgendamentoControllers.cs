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
    // Controller responsável por funcionalidades de agendamento:
    // - Listar horários disponíveis
    // - Validar datas/horários
    // - Salvar novos agendamentos
    public class AgendamentoController : Controller
    {
    
        private readonly AgendamentoRepositories _agendamentoRepository;
        private readonly AppDbContext _context;
        private readonly AgendamentoRepositories _repo;
        private readonly IConfiguration _configuration;
       
        // Construtor com injeção de dependências (repositório, configuração e contexto)
        public  AgendamentoController(AgendamentoRepositories agendamentoRepository, IConfiguration configuration, AppDbContext context)
        {
            _agendamentoRepository = agendamentoRepository;
            _configuration = configuration;
            _context= context;
        }

        [HttpPost]
        // API para criar um agendamento (usada por formulários/JS)
        public IActionResult Criar(Agendamento agendamento)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _agendamentoRepository.Adicionar(agendamento);

            return Ok(agendamento);
        }

            [Authorize]
            [HttpGet("~/Home/Agenda")]
            // Exibe a página de agendamento para uma data (GET)
            // Carrega horários disponíveis, quadras e datas bloqueadas
            public async Task<IActionResult> Agenda(DateTime? dataSelecionada)
            {
    
    DateTime data = dataSelecionada ?? DateTime.Today;
    
    // Buscar datas bloqueadas (não disponíveis) para desabilitar no calendário
    var datasBloqueadas = await _context.Diaquadras
        .Where(d => !d.Disponivel &&
                    d.Data >= DateTime.Today)
        .Select(d => d.Data.ToString("yyyy-MM-dd"))
        .Distinct()
        .ToListAsync();
    ViewBag.DatasBloqueadas = datasBloqueadas;
    
    // Validar se a data selecionada está bloqueada
    var dataBloqueada = await _context.Diaquadras
        .AnyAsync(d => d.Data.Date == data.Date && !d.Disponivel);
    
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
    
    // Se a data está bloqueada (folga), não mostrar horários
    if (dataBloqueada)
    {
        disponiveis.Clear();
    }
    
    // Filtrar horários após 10:00 nos domingos
    if (data.DayOfWeek == DayOfWeek.Sunday || data.DayOfWeek == DayOfWeek.Saturday)
    {
        disponiveis = disponiveis.Where(h => h < new TimeSpan(10, 0, 0)).ToList();
    }
    if(data.DayOfWeek == DayOfWeek.Monday || data.DayOfWeek == DayOfWeek.Tuesday || data.DayOfWeek == DayOfWeek.Wednesday || data.DayOfWeek == DayOfWeek.Thursday || data.DayOfWeek == DayOfWeek.Friday)
        {
            grandeTotal = grandeTotal.Where(h => h >= new TimeSpan(16, 0, 0)).ToList();
        }
    ViewBag.HorariosDisponiveis = disponiveis.Select(h => h.ToString(@"hh\:mm")).ToList();
    ViewBag.DataSelecionada = data.ToString("yyyy-MM-dd");
    var quadras = await _context.Quadras.ToListAsync();
ViewBag.Quadras = quadras;
    return View("~/Views/Home/Agenda.cshtml");
}

    [Authorize]
    [HttpPost("~/Home/Agenda")]
    // Recebe o POST do agendamento; valida disponibilidade e salva
    public async Task<IActionResult> Agenda(Agendamento model)
    {
var datasBloqueadas = await _context.Diaquadras
    .Where(d => !d.Disponivel &&
                d.Data >= DateTime.Today)
    .Select(d => d.Data.ToString("yyyy-MM-dd"))
    .ToListAsync();
    ViewBag.DatasBloqueadas = datasBloqueadas;

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
    
    // Validar se a data está bloqueada (folga)
    var dataBloqueada = await _context.Diaquadras
        .AnyAsync(d => d.Data.Date == data.Date && !d.Disponivel);
    
    // Se a data está bloqueada, não mostrar horários
    if (dataBloqueada)
    {
        disponiveis.Clear();
    }
    
    // Filtrar horários após 10:00 nos domingos
    if (data.DayOfWeek == DayOfWeek.Sunday || data.DayOfWeek == DayOfWeek.Saturday)
    {
        disponiveis = disponiveis.Where(h => h < new TimeSpan(10, 0, 0)).ToList();
    }
    if(data.DayOfWeek == DayOfWeek.Monday || data.DayOfWeek == DayOfWeek.Tuesday || data.DayOfWeek == DayOfWeek.Wednesday || data.DayOfWeek == DayOfWeek.Thursday || data.DayOfWeek == DayOfWeek.Friday)
        {
            grandeTotal = grandeTotal.Where(h => h >= new TimeSpan(18, 0, 0)).ToList();
        }
    ViewBag.HorariosDisponiveis = disponiveis.Select(h => h.ToString(@"hh\:mm")).ToList();
    ViewBag.DataSelecionada = data.ToString("yyyy-MM-dd");
    ViewBag.DatasBloqueadas = datasBloqueadas;
     ModelState.AddModelError("", "O horário selecionado já está ocupado. Por favor, escolha outro horário.");
     return View("~/Views/Home/Agenda.cshtml", model);
}
        [HttpGet("Agendamento/BuscarQuadras")]
        // Retorna JSON com as quadras — usado por chamadas AJAX no frontend
        public async Task<IActionResult> BuscarQuadras()
        {
            var Quadras = await _context.Quadras.ToListAsync();
            return Json(Quadras);
        }
        
        
       
    }

}