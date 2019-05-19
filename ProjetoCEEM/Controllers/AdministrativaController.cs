using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ProjetoCEEM.Models;
using ProjetoCEEM.Services;
using ProjetoCEEM.ViewModels;


namespace ProjetoCEEM.Controllers
{
    [Authorize]
    public class AdministrativaController : Controller
    {
        Context db = new Context();

        // GET: Administrativa
        public ActionResult Index()
        {
            //pega usuario logado
            var usuarioLogado = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));

            ViewBag.ContadorPreCadastro = db.Cadastros.Count(c => c.StatusCadastroId == 1);
            ViewBag.ContadorUsuario = db.Usuarios.Count();
            ViewBag.ContadorAgenda = db.Agendas.Count(c => c.Start >= DateTime.Now);
            ViewBag.StatusContaUsuario = usuarioLogado.StatusUsuario.Descricao;
            ViewBag.ContadorOS = db.OrdemServicos.Count(o => o.StatusOrdemServicoId == 1);
                       

            //vencimento
            if (db.Pagamentos.Where(t => t.UsuarioId == usuarioLogado.UsuarioId).FirstOrDefault() != null)
            {
                var vencimento = db.Pagamentos.OrderByDescending(p => p.DataVencimento).FirstOrDefault(p => p.UsuarioId == usuarioLogado.UsuarioId);
                ViewBag.Vencimento = vencimento.DataVencimento;
            }
            else
            {
                ViewBag.Vencimento = usuarioLogado.DataCadastro.AddDays(15);
            }
            

            //bandeira
            var bandeira = db.Bandeiras.Where(b => b.BandeiraVigente == true).FirstOrDefault();
            if (bandeira != null)
            {
                ViewBag.Cor = bandeira.Cor;
                ViewBag.Valor = bandeira.Valor;
                ViewBag.Descricao = bandeira.Descricao;
            }

            ViewData["Bandeira"] = bandeira.BandeiraId;


            //Tarifa
            int Id = usuarioLogado.CadastroId;
            var enderecoUsuarioLogado = db.Enderecos.Single(u => u.CadastroId == Id);
            var tarifa = db.TaxaTarifarias.OrderByDescending(os => os.Id).FirstOrDefault(c => c.Uf == enderecoUsuarioLogado.Uf);
            ViewBag.ValorTarifa = tarifa.Valor;
            ViewBag.DistribuidoraTarifa = tarifa.Distribuidora;


            var ano = DateTime.Now.Year;
            var mes = DateTime.Now.Month;
            var dia = DateTime.Now.Day;


            #region Gambiarra Temporaria
            var equipamentoGabiarra = db.Equipamentos.FirstOrDefault(e => e.UsuarioId == usuarioLogado.UsuarioId);
            #endregion

            #region Grafico de Pizza
            var pizza = GraficoService.GetGraficoPizza(dia, mes, ano, equipamentoGabiarra.EquipamentoId, db);
            ViewBag.GraficoPizza = pizza;
            ViewBag.DataGraficoPizza = DateTime.Now.ToShortDateString();
            #endregion

            #region Grafico de Barras Verticais
            var barrasVerticais = GraficoService.GetGraficoBarrasVerticais(ano, equipamentoGabiarra.EquipamentoId, db);
            ViewBag.GraficoBarrasVerticais = barrasVerticais;
            ViewBag.AnoBarrasVerticais = ano;
            #endregion
            return View();
        }

        public ActionResult PreCadastros()
        {
            var perguntaSegurancaCadastros = db.PerguntaSegurancaCadastros.ToList();
            return View(perguntaSegurancaCadastros);
        }

    }
}