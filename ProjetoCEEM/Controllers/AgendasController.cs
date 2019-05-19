using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoCEEM.Models;
using ProjetoCEEM.Services;

namespace ProjetoCEEM.Controllers
{
    [Authorize]
    public class AgendasController : Controller
    {
        Context db = new Context();
        // GET: Agendas
        public ActionResult Index()
        {
            //mostra clientes na dropdown
            //var Cliente = new SelectList(db.Cadastros.Where(c => c.StatusCadastroId ==1).ToList(), "CadastroId", "NomeCompleto");
            //ViewData["DBCadastros"] = Cliente;

            return View();
        }
                
        public JsonResult GetCliente(string tipo)
        {
            //selct *
            //IEnumerable<Cadastro> Clientes = from cli in db.Cadastros where cli.StatusCadastroId == t select cli;

            int t = Convert.ToInt32(tipo);

            //SELECIONA COLUNA ESPECIFICA
            //IEnumerable<Cadastro> Clientes = from cli in db.Cadastros where cli.StatusCadastroId == t select cli.NomeCompleto;

            IEnumerable<Cadastro> Clientes = from cli in db.Cadastros join os in db.OrdemServicos on cli.CadastroId equals os.CadastroId where cli.StatusCadastroId == t && os.StatusOrdemServicoId != 3 && os.StatusOrdemServicoId != 4 select cli;

            //permite retorno
            if (HttpContext.Request.IsAjaxRequest())
            {

                //sete coluna que deve mostrar
                return Json(new SelectList(Clientes, "CadastroId", "NomeCompleto"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetEvents()
        {
            var events = db.Agendas.ToList();
            return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [AcceptVerbs("GET","POST")]
        public JsonResult GetAgendamento(int agendaId)
        {
            var agenda = db.Agendas.Find(agendaId);
            return new JsonResult { Data = agenda, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SaveEvent(Agenda e)
        {
            var status = false;
            int cadastroId = Convert.ToInt32(e.Cliente); //pega id do cliente

            //Vai na tabela Contato procura o cliente(CadastroId == cadastroId) e filtra apenas o email dele (TipoContatoId == 1) 
            var cadastro = db.Contatos.Where(c => c.CadastroId == cadastroId && c.TipoContatoId == 1).FirstOrDefault();

            //pega usuario logado que esta marcando calendario
            var usuarioLogado = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));

            //procura o 1º cliente na ordem de serviço(sempre pega ultima os)
            var os = db.OrdemServicos.OrderByDescending(o => o.OrdemServicoId).FirstOrDefault(c => c.CadastroId != null && c.CadastroId == cadastroId);

            if (e.AgendaId > 0)
            {
                //Update the event
                var v = db.Agendas.Where(a => a.AgendaId == e.AgendaId).FirstOrDefault();
                if (v != null)
                {
                    v.OrdemServicoId = os.OrdemServicoId;
                    v.Cliente = cadastro.Cadastro.NomeCompleto;
                    v.Start = e.Start;
                    v.Description = e.Description;
                    v.ThemeColor = e.ThemeColor;
                    v.TipoAgendamento = e.TipoAgendamento;
                }
            }
            else
            {
                e.OrdemServicoId = os.OrdemServicoId;
                e.Cliente = cadastro.Cadastro.NomeCompleto;
                db.Agendas.Add(e);
            }

            if (os != null)
            {
                os.DataAlteracao = DateTime.Now;
                os.UsuarioCriacaoId = usuarioLogado.CadastroId; //pega usuario logado
                os.StatusOrdemServicoId = 2; //Seta Status da OS para "Em Andamento"
                os.UsuarioAtendenteId = usuarioLogado.CadastroId; // add quem atendeu
                db.Entry(os).State = EntityState.Modified; //salva Ordem de serviço
            }

            db.SaveChanges();
            status = true;

            var emailEnviado = EmailService.EnviarEmail(cadastro.Descricao, "Agendamento Projeto CEEM",
                "Olá " + cadastro.Cadastro.NomeCompleto + ", foi agendado um visita técnica para você dia: " + e.Start);

            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int agendaId)
        {
            var status = false;

            var v = db.Agendas.Where(a => a.AgendaId == agendaId).FirstOrDefault();
            if (v != null)
            {
                db.Agendas.Remove(v);
                db.SaveChanges();
                status = true;
            }

            return new JsonResult { Data = new { status = status } };
        }
    }
}