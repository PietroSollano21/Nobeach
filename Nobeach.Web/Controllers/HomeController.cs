using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nobeach.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Nobeach.Data;
using System.Data;
using Nobeach.Enums;

namespace Nobeach.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public HomeController(AppDbContext context, IConfiguration configuration)
{
    _context = context;
    _configuration = configuration;
}
    [HttpGet]
    public IActionResult Index()
    {
       
        if(User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Privacy");
        }

        return View();
    }

    public async Task<IActionResult> Privacy()
    {
         if(User.Identity.IsAuthenticated && User.IsInRole("Admin"))
        {
            return RedirectToAction("Admin", "Adm");
        }
         if (User == null || User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
        {
            return RedirectToAction("Login", "Usuario");
        }
        string emailLogado = User.Identity.Name;
        Console.WriteLine($"Email logado:{emailLogado}");
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == emailLogado);
        Console.WriteLine(usuario == null ? "Usuário não encontrado" : $"Usuário encontrado: {usuario.Email}");
         if (usuario == null)
        {
            return RedirectToAction("Login", "Usuario");
        }
        if (usuario == null)
        {
            return RedirectToAction("Login", "Usuario");
        }
        DateTime dataHoje = DateTime.Today;
       var meusAgendamentos = await _context.Agendamentos
    .Where(a => a.EmailCliente == usuario.Email)
    .Where(a => a.Data.Date >= dataHoje)
    .OrderBy(a => a.Data)
    .ThenBy(a => a.Hora)
    .ToListAsync();
    return View(meusAgendamentos);
    }
    public IActionResult Agenda()
    {
        ViewBag.Quadras= _context.Quadras.ToList();
        return View();
    }
     [HttpPost]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ConfirmarAgendamento(
    string nome,
    string data,
    string hora,
    string quadra,
    Esporte esporte)
{
    // Validação básica
    if (string.IsNullOrWhiteSpace(nome) ||
        string.IsNullOrWhiteSpace(data) ||
        string.IsNullOrWhiteSpace(hora) ||
        string.IsNullOrWhiteSpace(quadra))
    {
        TempData["Erro"] = "Preencha todos os campos.";
        return RedirectToAction("Agenda");
    }

    DateTime dataAgendamento;
    TimeSpan horaAgendamento;

    if (!DateTime.TryParse(data, out dataAgendamento))
    {
        TempData["Erro"] = "Data inválida.";
        return RedirectToAction("Agenda");
    }

    if (!TimeSpan.TryParse(hora, out horaAgendamento))
    {
        TempData["Erro"] = "Horário inválido.";
        return RedirectToAction("Agenda");
    }

    // Não permitir datas passadas
    if (dataAgendamento.Date < DateTime.Today)
    {
        TempData["Erro"] = "Não é possível agendar datas passadas.";
        return RedirectToAction("Agenda");
    }

    // Verifica se a quadra está bloqueada para a data
    bool quadraBloqueada = await _context.Diaquadras.AnyAsync(d =>
        d.QuadraId == quadra &&
        d.Data.Date == dataAgendamento.Date &&
        !d.Disponivel);

    if (quadraBloqueada)
    {
        TempData["Erro"] = "Esta quadra está indisponível nesta data.";
        return RedirectToAction("Agenda");
    }

    // Verifica se já existe reserva para a mesma quadra no mesmo horário
    bool horarioOcupado = await _context.Agendamentos.AnyAsync(a =>
        a.Quadra == quadra &&
        a.Data.Date == dataAgendamento.Date &&
        a.Hora == horaAgendamento &&
        a.Status != "Cancelado");

    if (horarioOcupado)
    {
        TempData["Erro"] = "Este horário já está reservado.";
        return RedirectToAction("Agenda");
    }

    string emailCliente = User.Identity?.Name ?? "";

    var novoAgendamento = new Agendamento
    {
        NomeCliente = nome,
        Data = dataAgendamento,
        Hora = horaAgendamento,
        Quadra = quadra,
        Esporte = esporte,
        EmailCliente = emailCliente,
        Status = "Agendado"
    };

    _context.Agendamentos.Add(novoAgendamento);
    await _context.SaveChangesAsync();

    TempData["Sucesso"] = "Agendamento realizado com sucesso!";

    return RedirectToAction("Privacy", "Home");
}
[HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarAgendamentos(long? id)
    {
        Console.WriteLine("CHEGOU");
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento == null)
        {
            return NotFound();
        }
        if(agendamento.Cancelado )
        {
            TempData["Erro"] = "Nao é possível cancelar um agendamento com menos de 6 horas de antecedência.";
            return RedirectToAction("Privacy");
        }
                _context.Agendamentos.Remove(agendamento);
         
            await _context.SaveChangesAsync();
            TempData["Sucesso"]= "Agendamento cancelado!";

        return RedirectToAction("Privacy");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
