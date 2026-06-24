using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nobeach.Data;
using Nobeach.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Nobeach.Controllers
{
    // Controller administrativo — gerencia o painel do admin e configurações de dias
    public class AdmController : Controller
    {
        private readonly AppDbContext _context;
        public AdmController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        // Página principal do administrador: lista agendamentos e dias configurados
        public async Task<IActionResult> Admin()
        {
            var admLogado = User.Identity?.Name;
var usuario = await _context.Usuarios
    .FirstOrDefaultAsync(u => u.Email == admLogado);
            if (usuario == null || usuario.Perfil != "Admin")            {
                return RedirectToAction("Index", "Home");
            }

            DateTime dataHoje = DateTime.Today;
            //var agendamentos = await _context.Agendamentos.Where(a => (a.statuspagamento == "Pagar na hora" ||  a.statuspagamento == "Pago") && a.BarbeiroNome == usuario.Nome).Where(a => a.Data.Date >= dataHoje.Date).OrderBy(a => a.Data).ThenBy(a => a.Hora).ToListAsync();
    var agendamentos = await _context.Agendamentos.Where(a => a.Data.Date >= dataHoje.Date).OrderBy(a => a.Data).ThenBy(a => a.Hora).ToListAsync();
    var diasQuadra = await _context.Diaquadras.Where(d => d.Data >= dataHoje).OrderBy(d => d.Data).ToListAsync();
    return View((agendamentos, diasQuadra));
        }
        
        // View para configurar dias (folgas / dias disponíveis)
        public async Task<IActionResult> ConfigurarDias()
    {

        var datasconfig = _context.Diaquadras.Where(d => d.Data >= DateTime.Today).OrderBy(d => d.Data).ToList();
        var agendamentos = _context.Agendamentos.Where(a => a.Data >= DateTime.Today).ToList();
        return View(datasconfig);
    }
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    // Recebe a data e o status (Disponível / Folga) para gravar no banco.
    // Se marcar como folga, cancela agendamentos existentes para essa data.
    public async Task<IActionResult> DataExata(DateTime novaData, string status)
    {
        if(novaData == DateTime.MinValue || string.IsNullOrEmpty(status))
        {
            return BadRequest("Erro de Binding: Os dados do formulário não chegaram no Controller. Verifique os nomes (name=) no HTML.");
        }
       var QuadraId = User.Identity.Name;
       bool vaiTrabalhar = status == "Disponível";
       bool folga = status == "Folga";
       Console.WriteLine($"QuadraId salvo: {User.Identity.Name}");
         var dataExistente = await _context.Diaquadras.FirstOrDefaultAsync(d =>d.Data.Date == novaData.Date);
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
            _context.Diaquadras.Update(dataExistente);
        }
        else
        {
            var novaConfig = new Diaquadra
            {
                QuadraId = QuadraId,
                Data = novaData.Date,
                Disponivel = vaiTrabalhar
            };
            await _context.Diaquadras.AddAsync(novaConfig);
        }
        var alteraçoes = await _context.SaveChangesAsync();
        Console.WriteLine($"Alterações salvas: {alteraçoes}");
        TempData["Sucesso"] = vaiTrabalhar ? $"Esse dia {novaData.Date} foi configurado como disponível." : $"Esse dia {novaData.Date} foi configurado como folga.";
        return RedirectToAction("Admin");
        
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    // Cancela uma folga existente (marca como disponível)
    public async Task<IActionResult> CancelarFolga(DateTime data)
    {
        var folga = await _context.Diaquadras.FirstOrDefaultAsync(d => d.Data.Date == data.Date && !d.Disponivel);
        
        if (folga != null)
        {
            folga.Disponivel = true;
            _context.Diaquadras.Update(folga);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = $"Folga de {data.Date:dd/MM/yyyy} foi cancelada.";
        }
        else
        {
            TempData["Erro"] = "Folga não encontrada.";
        }
        
        return RedirectToAction("Admin");
    }   
}
}