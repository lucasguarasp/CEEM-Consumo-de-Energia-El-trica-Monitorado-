using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjetoCEEM.Models;
using ProjetoCEEM.Services;
using ProjetoCEEM.ViewModels;

namespace ProjetoCEEM.Controllers
{
    public class UsuariosController : Controller
    {
        Context db = new Context();

        // GET: Usuarios
        [Authorize(Roles = "Administrador, Desenvolvedor")]
        public ActionResult UsuariosRegistrados()
        {
            var listaUsuariosRegistrados = db.Usuarios.ToList();
            return View(listaUsuariosRegistrados);
        }

        [HttpPost]
        public JsonResult EditarCadastro(string codigo, string nome, string email, string opcao)
        {
            var status = false;
            int cod = Convert.ToInt32(codigo);
            var usu = db.Cadastros.Where(c => c.CadastroId == cod).FirstOrDefault();
            var contatoEmail = db.Usuarios.Where(t => t.CadastroId == cod).FirstOrDefault();

            var usuarioPerfils = db.UsuarioPerfis.Where(t => t.Usuario.CadastroId == cod).FirstOrDefault();
            if (usuarioPerfils != null)
            {
                if (opcao == "1" || opcao == "2")
                {
                    int op = Convert.ToInt32(opcao);
                    usuarioPerfils.PerfilId = op;
                    db.Entry(usuarioPerfils).State = System.Data.Entity.EntityState.Modified;

                }
            }
            if (usu != null)
            {
                contatoEmail.Email = email;
                usu.NomeCompleto = nome;
                db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                db.Entry(contatoEmail).State = System.Data.Entity.EntityState.Modified;



            }
            status = true;
            db.SaveChanges();
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult EditarPreCadastro(string codigo, string nome, string email, string cep, string rua,
            string bairro, string numero, string complemento, string cidade, string uf,
            string celular, string telefone, DateTime dataNascimento)
        {
            var status = false;
            //cod pega o id do cliente
            int cod = Convert.ToInt32(codigo);

            var cad = db.Cadastros.Where(t => t.CadastroId == cod).FirstOrDefault();

            //pega os contatos do clientes
            var contatoEmail = db.Contatos.Where(t => t.CadastroId == cod && t.TipoContatoId == 1).FirstOrDefault();
            var contatoTelefone = db.Contatos.Where(t => t.CadastroId == cod && t.TipoContatoId == 2).FirstOrDefault();
            var contatoCelular = db.Contatos.Where(t => t.CadastroId == cod && t.TipoContatoId == 3).FirstOrDefault();

            var end = db.Enderecos.Where(t => t.CadastroId == cod).FirstOrDefault();

            //verifa se exite o cadastro com id do cliente p/ fazer as alterações
            if (cad != null)
            {
                cad.NomeCompleto = nome;
                cad.DataNascimento = dataNascimento;

                end.Cep = cep;
                end.Rua = rua;
                end.Bairro = bairro;
                end.NumeroCasa = numero;
                end.Complemento = complemento;
                end.Cidade = cidade;
                end.Uf = uf;

                contatoEmail.Descricao = email;

                if (contatoTelefone != null)
                {
                    contatoTelefone.Descricao = telefone;
                    db.Entry(contatoTelefone).State = System.Data.Entity.EntityState.Modified;
                }
                if (contatoCelular != null)
                {
                    contatoCelular.Descricao = celular;
                    db.Entry(contatoCelular).State = System.Data.Entity.EntityState.Modified;
                }

                db.Entry(cad).State = System.Data.Entity.EntityState.Modified;
                db.Entry(contatoEmail).State = System.Data.Entity.EntityState.Modified;
                db.Entry(end).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                status = true;
            }

            return new JsonResult { Data = new { status = status } };
        }
        //Get PreCadastro
        [Authorize(Roles = "Administrador, Desenvolvedor")]
        public ActionResult PreCadastros()
        {

            List<Cadastro> listaCadastros = db.Cadastros.Where(c => c.StatusCadastroId == 1 && c.StatusCadastroId != 3).ToList();

            var listaPreCadastros = new List<EditPrecadastroViewModel>();
            if (listaCadastros.Count > 0)
                foreach (var cadastro in listaCadastros)
                {
                    var endereco = db.Enderecos.Single(e => e.CadastroId == cadastro.CadastroId);
                    var preCadastro = new EditPrecadastroViewModel
                    {
                        CadastroId = cadastro.CadastroId,
                        NomeCompleto = cadastro.NomeCompleto,
                        DataNascimento = cadastro.DataNascimento,
                        Cep = endereco.Cep,
                        Rua = endereco.Rua,
                        Bairro = endereco.Bairro,
                        Cidade = endereco.Cidade,
                        Uf = endereco.Uf,
                        Numero = endereco.NumeroCasa,
                        Complemento = endereco.Complemento,
                        DataCriacao = cadastro.DataCriacao

                    };

                    if (db.Contatos.Single(e => e.CadastroId == cadastro.CadastroId && e.TipoContatoId == 1) != null)
                    {
                        Contato emailContato = db.Contatos.Single(e => e.CadastroId == cadastro.CadastroId && e.TipoContatoId == 1);
                        preCadastro.Email = emailContato.Descricao;
                    }
                    if (db.Contatos.Count(e => e.CadastroId == cadastro.CadastroId && e.TipoContatoId == 3) > 0)
                    {
                        Contato celularContato = db.Contatos.Single(e => e.CadastroId == cadastro.CadastroId && e.TipoContatoId == 3);
                        preCadastro.Celular = celularContato.Descricao;

                    }
                    if (db.Contatos.Count(e => e.CadastroId == cadastro.CadastroId && e.TipoContatoId == 2) > 0)
                    {
                        Contato telefoneContato = db.Contatos.Single(e => e.CadastroId == cadastro.CadastroId && e.TipoContatoId == 2);
                        preCadastro.Telefone = telefoneContato.Descricao;

                    }

                    //pega status da OS do cliente
                    preCadastro.OrdemServico = db.OrdemServicos.Single(o => o.CadastroId == preCadastro.CadastroId);

                    listaPreCadastros.Add(preCadastro);
                }


            return View(listaPreCadastros);
        }

        //Bloquear / Ativar usuario
        public ActionResult EditarStatusCadastro(int id, string status)
        {
            var usu = db.Usuarios.Where(c => c.CadastroId == id).FirstOrDefault();

            if (usu != null)
            {

                if (status == "1")
                {
                    usu.StatusUsuarioId = 1;
                    db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                }

                if (status == "0")
                {
                    usu.StatusUsuarioId = 3;
                    db.Entry(usu).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges();

            }

            return RedirectToAction("UsuariosRegistrados", "Usuarios");
        }

        //modificar status Ordem de Serviço
        public ActionResult EditarStatusOS(int id, string StatusOS)
        {
            var usuarioLogado = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var cadastro = db.Cadastros.Where(c => c.CadastroId == id).FirstOrDefault();
            var os = db.OrdemServicos.Where(c => c.CadastroId == id).FirstOrDefault();
            var contato = db.Contatos.Where(c => c.CadastroId == id && c.TipoContatoId == 1).FirstOrDefault();

            if (os != null)
            {
                if (StatusOS == "4" && (os.StatusOrdemServicoId == 2 || os.StatusOrdemServicoId == 1))
                {
                    //enviar email para cliente sobre o cancelamento
                    os.StatusOrdemServicoId = 4;
                    cadastro.StatusCadastroId = 3;
                    os.DataAlteracao = DateTime.Now;
                    os.UsuarioAtendenteId = usuarioLogado.CadastroId;
                    db.Entry(os).State = System.Data.Entity.EntityState.Modified;

                    var emailEnviado = EmailService.EnviarEmail(contato.Descricao, "Cancelamento Projeto CEEM",
                "Olá " + os.Cadastro.NomeCompleto + ", Não foi possível concluir sua ordem de serviço de nº: " + os.NumeroOS + ", concluímos que, não terá custo alagum.");

                }

                if (StatusOS == "3" && os.StatusOrdemServicoId == 2)
                {
                    //enviar hash para concluir cadastro
                    os.StatusOrdemServicoId = 3;
                    os.UsuarioAtendenteId = usuarioLogado.CadastroId;
                    db.Entry(os).State = System.Data.Entity.EntityState.Modified;
                    var callbackUrl = Url.Action(
                        "ConcluirCadastro", "Cadastros",
                        new { CadastroId = HashService.Crip(os.CadastroId.ToString()) },
                        protocol: Request.Url.Scheme);

                    EmailService.EnviarEmail(contato.Descricao, "Conclua seu cadastro no Projeto CEEM",
                        "Olá "
                        + os.Cadastro.NomeCompleto
                        + ", Seu cadastro foi aprovado, click no <a href = \"" + callbackUrl + "\" >link</a> para completar as informações e concluir seu cadastro. <p> Não esqueça de preencher os campos LOGIN E SENHA </p>");

                }

                if (ModelState.IsValid)
                {
                    TempData["message"] = "Ordem serviço " + os.StatusOrdemServico.Descricao.ToUpper();
                }

                db.SaveChanges();
            }
            return RedirectToAction("PreCadastros", "Usuarios");            
        }

    }
}