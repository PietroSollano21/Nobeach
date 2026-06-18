using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nobeach.Data;
using Nobeach.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Barbearia.Controllers
{
    public class AdmController : Controller
    {
        private readonly AppDbContext _context;
        public AdmController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var barbeiroLogado = User.Identity?.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == barbeiroLogado);
            if (usuario == null || usuario.Perfil != "Admin")            {
                return RedirectToAction("Login", "Usuario");
            }
            DateTime dataHoje = DateTime.Today;
            //var agendamentos = await _context.Agendamentos.Where(a => (a.statuspagamento == "Pagar na hora" ||  a.statuspagamento == "Pago") && a.BarbeiroNome == usuario.Nome).Where(a => a.Data.Date >= dataHoje.Date).OrderBy(a => a.Data).ThenBy(a => a.Hora).ToListAsync();
    var agendamentos = await _context.Agendamentos.Where(a => a.Data.Date >= dataHoje.Date).OrderBy(a => a.Data).ThenBy(a => a.Hora).ToListAsync();
    return View(agendamentos);
        }
        
public async Task<IActionResult> ConfigurarDias()
    {

        var barbeiroid = User.Identity?.Name;
        var datasconfig = _context.Diaquadras.Where(d => d.BarbeiroId == barbeiroid && d.Data >= DateTime.Today).OrderBy(d => d.Data).ToList();
        var agendamentos = _context.Agendamentos.Where(a => a.Data >= DateTime.Today).ToList();
        return View(datasconfig);
    }
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DataExata(DateTime novaData, string status)
    {
        if(novaData == DateTime.MinValue || string.IsNullOrEmpty(status))
        {
            return BadRequest("Erro de Binding: Os dados do formulário não chegaram no Controller. Verifique os nomes (name=) no HTML.");
        }
       var barbeiroId = User.Identity.Name;
       bool vaiTrabalhar = status == "Disponível";
       bool folga = status == "Folga";
       Console.WriteLine($"BarbeiroId salvo: {User.Identity.Name}");
         var dataExistente = await _context.Diaquadras.FirstOrDefaultAsync(d => d.BarbeiroId == barbeiroId && d.Data.Date == novaData.Date);
    if(!vaiTrabalhar)
            {
                //var agendamentodia = await _context.Agendamentos.Where(a => a.Data.Date == novaData.Date && (a.statuspagamento == "Pagar na hora" || a.statuspagamento == "Pago")).ToListAsync();
            var agendamentodia = await _context.Agendamentos.Where(a => a.Data.Date == novaData.Date).ToListAsync();
            foreach (var agendamento in agendamentodia)
                {
                    agendamento.Status = "Cancelado";
                    _context.Agendamentos.Update(agendamento);
                }
            }
        if (dataExistente != null)
        {
            dataExistente.Disponivel = vaiTrabalhar;
            _context.DiaBarbeiros.Update(dataExistente);
        }
        else
        {
            var novaConfig = new DiaBarbeiro
            {
                BarbeiroId = barbeiroId,
                Data = novaData.Date,
                Disponivel = vaiTrabalhar
            };
            await _context.DiaBarbeiros.AddAsync(novaConfig);
        }
        var alteraçoes = await _context.SaveChangesAsync();
        Console.WriteLine($"Alterações salvas: {alteraçoes}");
        TempData["Sucesso"] = vaiTrabalhar ? $"Esse dia {novaData.Date} foi configurado como disponível." : $"Esse dia {novaData.Date} foi configurado como folga.";
        return RedirectToAction("Barbeiro");
        
    }   
}
}