using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoCEEM.ViewModels;
using ProjetoCEEM.Models;
using ProjetoCEEM.Services;
using System.Data.Entity;

namespace ProjetoCEEM.Controllers
{
    [Authorize]
    public class PrivacidadeController : Controller
    {
        Context db = new Context();
        // GET: Privacidade
        public ActionResult AlterarSenha()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlterarSenha(PrivacidadeSenhaViewModel senhaViewModel, int InputForcaSenha)
        {
            var valido = true;
            var usuario = db.Usuarios.Single(u => u.Login == User.Identity.Name);
            if (InputForcaSenha < 60)
            {
                ModelState.AddModelError("NovaSenha", "É necessario ter uma senha Forte");
                valido = false;
            }
            if (!ModelState.IsValid) return View(senhaViewModel);
            if (!HashService.Crip(senhaViewModel.Senha).Equals(usuario.Senha))
            {
                ModelState.AddModelError("Senha", "Não há correspondencia entre a senha cadastrada para o usuário e a senha informada");
                valido = false;
            }
            if (!valido) return View(senhaViewModel);
            usuario.Senha = HashService.Crip(senhaViewModel.NovaSenha);
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            return View();
        }
        public ActionResult PerguntaSeguranca()
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var perguntaSegurancaUsuario = db.PerguntaSegurancaCadastros.Find(usuario.CadastroId);
            if (perguntaSegurancaUsuario == null) return HttpNotFound();
            var perguntaSeguranca = new PrivacidadePerguntaSegurancaViewModel
            {
                PerguntaSegurancaList = new SelectList(db.PerguntaSegurancas, "PerguntaSegurancaId",
                    "Descricao", perguntaSegurancaUsuario.PerguntaSeguracaId),
                Resposta = perguntaSegurancaUsuario.Resposta,
                PerguntaSegurancaId = perguntaSegurancaUsuario.PerguntaSeguracaId
            };
            return View(perguntaSeguranca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PerguntaSeguranca(PrivacidadePerguntaSegurancaViewModel perguntaSegurancaViewModel)
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var perguntaSegurancaUsuario = db.PerguntaSegurancaCadastros.Find(usuario.CadastroId);
            if (perguntaSegurancaUsuario == null) return HttpNotFound();
            var perguntaSeguranca = new PrivacidadePerguntaSegurancaViewModel
            {
                PerguntaSegurancaList = new SelectList(db.PerguntaSegurancas, "PerguntaSegurancaId",
                    "Descricao", perguntaSegurancaUsuario.PerguntaSeguracaId),
                Resposta = perguntaSegurancaUsuario.Resposta,
                PerguntaSegurancaId = perguntaSegurancaUsuario.PerguntaSeguracaId
            };
            perguntaSegurancaUsuario.PerguntaSeguracaId = perguntaSegurancaViewModel.PerguntaSegurancaId;
            perguntaSegurancaUsuario.Resposta = perguntaSegurancaViewModel.Resposta;
            db.Entry(perguntaSegurancaUsuario).State = EntityState.Modified;
            db.SaveChanges();
            return View(perguntaSeguranca);
        }

        public ActionResult EmailRecuperarSenha()
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var emailRecuperarSenha = new PrivacidadeEmailRecuperarSenha
            {
                Email = usuario.Email
            };
            return View(emailRecuperarSenha);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailRecuperarSenha(PrivacidadeEmailRecuperarSenha emailRecuperarSenhaViewModel)
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var emailRecuperarSenha = new PrivacidadeEmailRecuperarSenha
            {
                Email = usuario.Email
            };
            usuario.Email = emailRecuperarSenhaViewModel.Email;
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            return View(emailRecuperarSenha);
        }
    }
}