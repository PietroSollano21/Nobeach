using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nobeach.Models;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Nobeach.Data;
using System.Data;

namespace Nobeach.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
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
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == emailLogado);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
