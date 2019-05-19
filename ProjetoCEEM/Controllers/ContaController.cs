using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoCEEM.Models;
using ProjetoCEEM.ViewModels;

namespace ProjetoCEEM.Controllers
{
    [Authorize]
    public class ContaController : Controller
    {
        Context db = new Context();
        // GET: COnta
        public ActionResult InformacaoPessoal()
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var cadastro = db.Cadastros.Find(usuario.CadastroId);
            if (cadastro == null) return HttpNotFound("ocorreu um erro sem tratamento");
            var informacaoPessoal = new InformacaoPessoalViewModel
            {
                NomeCompleto = cadastro.NomeCompleto,
                DataNascimento = cadastro.DataNascimento
            };

            var perfis = "";
            foreach (var usuarioPerfil in db.UsuarioPerfis.Where(u => u.UsuarioId == usuario.UsuarioId))
            {
                using (var dbContext = new Context())
                    perfis += dbContext.Perfis.Find(usuarioPerfil.PerfilId)?.Descricao + ",";
            }

            perfis = perfis.Replace(",", " | ");
            ViewBag.Perfil = perfis.Remove(perfis.Length - 2);

            ViewBag.ImagemPerfil = @"~/Content/ImagemPerfil/" + usuario.NomeImagem;

            if (string.IsNullOrWhiteSpace(usuario.NomeImagem))
            {
                ViewBag.ImagemPerfil = @"~/Content/ImagemPerfil/profile.png";
            }

            return View(informacaoPessoal);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InformacaoPessoal(HttpPostedFileBase file, InformacaoPessoalViewModel informacaoPessoal)
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var cadastro = db.Cadastros.Find(usuario.CadastroId);

            ViewBag.Imagem = @"~/Content/ImagemPerfil/" + usuario.NomeImagem;
            ViewBag.ImagemPerfil = @"~/Content/ImagemPerfil/" + usuario.NomeImagem;

            var perfis = "";
            foreach (var usuarioPerfil in db.UsuarioPerfis.Where(u => u.UsuarioId == usuario.UsuarioId))
            {
                using (var dbContext = new Context())
                    perfis += dbContext.Perfis.Find(usuarioPerfil.PerfilId)?.Descricao + ",";
            }

            perfis = perfis.Replace(",", " | ");
            ViewBag.Perfil = perfis.Remove(perfis.Length - 2);

            var valido = true;
            if (!ModelState.IsValid)
                return View(informacaoPessoal);

            if (DateTime.Compare(informacaoPessoal.DataNascimento, DateTime.Now.AddYears(-18)) >= 0)
            {
                ModelState.AddModelError("DataNascimento", "É necessario ter mais de 18 anos para se cadastrar");
                valido = false;
            }

            if (!valido) return View(informacaoPessoal);
            if (cadastro != null)
            {
                cadastro.DataNascimento = informacaoPessoal.DataNascimento;
                cadastro.NomeCompleto = informacaoPessoal.NomeCompleto;

                db.Entry(cadastro).State = EntityState.Modified;
            }
            db.SaveChanges();
            ViewBag.Imagem = @"~/Content/ImagemPerfil/" + usuario.NomeImagem;
            ViewBag.ImagemPerfil = @"~/Content/ImagemPerfil/" + usuario.NomeImagem;
            return View(informacaoPessoal);

        }

        public ActionResult Endereco()
        {
            var login = User.Identity.Name;
            var usuario = db.Usuarios.Single(u => u.Login.Equals(login));
            var cadastro = db.Cadastros.Find(usuario.CadastroId);
            var endereco = db.Enderecos.Single(e => e.CadastroId == cadastro.CadastroId);
            return View(endereco);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Endereco(Endereco enderecoNovo)
        {
            var login = User.Identity.Name;
            var usuario = db.Usuarios.Single(u => u.Login.Equals(login));
            var cadastro = db.Cadastros.Find(usuario.CadastroId);
            var endereco = db.Enderecos.Single(e => e.CadastroId == cadastro.CadastroId);

            endereco.Cep = enderecoNovo.Cep;
            endereco.Rua = enderecoNovo.Rua;
            endereco.Bairro = enderecoNovo.Bairro;
            endereco.NumeroCasa = enderecoNovo.NumeroCasa;
            endereco.Complemento = enderecoNovo.Complemento;
            endereco.Cidade = enderecoNovo.Cidade;
            endereco.Uf = enderecoNovo.Uf;
            endereco.DataAlteracao = DateTime.Now;

            var valido = true;

            if (!CidadeAtendida.AtendeCidade(db, endereco.Cidade, endereco.Uf))
            {
                ModelState.AddModelError("Cidade", "Atualmente não atendemos a cidade informada");
                valido = false;
            }
            if (!CidadeAtendida.AtendeUf(db, endereco.Uf))
            {
                ModelState.AddModelError("Uf", "Atualmente não atendemos a região informada");
                valido = false;
            }
            if (!valido)
                return View(endereco);

            db.Entry(endereco).State = EntityState.Modified;
            db.SaveChanges();

            return View(endereco);
        }

        public ActionResult Contatos()
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var cadastro = db.Cadastros.Find(usuario.CadastroId);
            if (cadastro == null) return HttpNotFound();
            var contatosViewModel = new ContatosViewModel();
            contatosViewModel.Email = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 1).Descricao;
            contatosViewModel.Telefone = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 2).Descricao;
            contatosViewModel.Celular = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 3).Descricao;

            return View(contatosViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contatos(ContatosViewModel contatosNovo)
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var cadastro = db.Cadastros.Find(usuario.CadastroId);
            if (cadastro == null) return HttpNotFound();
            var email = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 1);
            var telefone = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 2);
            var celular = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 3);

            email.Descricao = contatosNovo.Email;
            telefone.Descricao = contatosNovo.Telefone;
            celular.Descricao = contatosNovo.Celular;

            db.Entry(email).State = EntityState.Modified;
            db.Entry(telefone).State = EntityState.Modified;
            db.Entry(celular).State = EntityState.Modified;

            db.SaveChanges();
            return View(contatosNovo);
        }
        [HttpPost]
        public ActionResult Upload()
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var file = Request.Files[0];
            if (file == null) return RedirectToAction("InformacaoPessoal", "Conta");

            try
            {
                if (ModelState.IsValid)
                {
                    string retorno = Services.Funcoes.Upload(file);
                    if (retorno.Contains('|'))
                    {
                        usuario.NomeImagem = retorno.Split('|')[1];
                    }
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                ViewBag.fileMensagem = "Não foi possível salvar a imagem: "+ e.Message.ToString();
            }

            return RedirectToAction("InformacaoPessoal", "Conta");
        }
    }
}