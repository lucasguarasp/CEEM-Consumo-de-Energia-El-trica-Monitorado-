using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ProjetoCEEM.Models;
using ProjetoCEEM.Services;
using ProjetoCEEM.ViewModels;

namespace ProjetoCEEM.Controllers
{
    public class UsuarioController : Controller
    {
        Context db = new Context();

        public ActionResult RecuperarSenha(string recuperarSenha)
        {
            if (recuperarSenha == null) return HttpNotFound("Codigo de recuperar senha não existe ou já foi usado");
            var codigo = Guid.Parse(recuperarSenha);
            Usuario usuario;
            if (db.Usuarios.Count(u => u.RecuperaSenha.ToString().Equals(codigo.ToString())) > 0)
                usuario = db.Usuarios.Single(u => u.RecuperaSenha.ToString().Equals(codigo.ToString()));
            else
                return HttpNotFound("Codigo de recuperar senha não existe ou já foi usado");
            var cadatro = db.Cadastros.Find(usuario.CadastroId);
            if (cadatro == null) return HttpNotFound("Codigo de recuperar senha não existe ou já foi usado");
            var recuperarSenhaViewModel = new RecuperarSenhaViewModel
            {
                Pergunta = usuario.Cadastro.PerguntaSegurancaCadastro.PerguntaSeguranca.Descricao
            };
            TempData["Usuario"] = usuario;
            db.Dispose();
            return View(recuperarSenhaViewModel);
        }
        // GET: Usuario
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recuperarSenhaViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarSenha(RecuperarSenhaViewModel recuperarSenhaViewModel, int InputForcaSenha)
        {
            if (InputForcaSenha < 60)
            {
                ModelState.AddModelError("Senha", "É necessario ter uma senha com força minima Forte");
            }
            if (!ModelState.IsValid) return View(recuperarSenhaViewModel);
            var usuario = (Usuario)TempData["Usuario"];
            if (!usuario.Cadastro.PerguntaSegurancaCadastro.Resposta.Equals(
                recuperarSenhaViewModel.Resposta))
            {
                TempData["Usuario"] = usuario;
                ModelState.AddModelError("Resposta", "Resposta errada");
                return View(recuperarSenhaViewModel);
            }

            usuario.Senha = HashService.Crip(recuperarSenhaViewModel.Senha);
            usuario.RecuperaSenha = null;
            usuario.DataDesbloqueio = null;
            usuario.QuantTentativas = 0;
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Login", "Usuario");
        }
        public ActionResult EsqueciSenha()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EsqueciSenha(UsuarioEsqueciSenhaViewModel esqueciSenha)
        {
            if (!ModelState.IsValid) return View(esqueciSenha);
            var usuario = esqueciSenha.RecuperarUsuarioEmail(db);
            if (usuario == null)
            {
                ModelState.AddModelError("Email", "Não existe usuário com o email correspondente");
                return View(esqueciSenha);
            }
            var guid = Guid.NewGuid();
            usuario.RecuperaSenha = guid;
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            if (!EmailService.GetSetting<bool>("EnviarEmail")) return RedirectToAction("Index", "Home");
            var callbackUrl = Url.Action(
                "RecuperarSenha", "Usuario",
                new { recuperarSenha = usuario.RecuperaSenha },
                protocol: Request.Url.Scheme);
            var emailEnviado = EmailService.EnviarEmail(esqueciSenha.Email, "Recupere sua conta do Projeto CEEM",
                "Acesse este " + "<a href = \"" + callbackUrl + "\" >link </a>" + " para alterar sua senha."
            );
            if (emailEnviado) return RedirectToAction("Index", "Home");
            ModelState.AddModelError("Email", string.Concat(EmailService.Msg, ", Erro: ", EmailService.MsgError));
            return View(esqueciSenha);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioLoginViewModel usuarioLogin, string returnUrl)
        {
            var usuario = usuarioLogin.Entrar(db);
            if (!ModelState.IsValid) return View(usuarioLogin);
            if (usuario == null)
            {
                ModelState.AddModelError("Login", "Usuário não existe");
                return View(usuarioLogin);

            }

            if (usuario.StatusUsuarioId == 3)
            {
                ModelState.AddModelError("Login", "Usuário Bloqueado");
                return View(usuarioLogin);
            }

            if (usuario.DataDesbloqueio != null && usuario.DataDesbloqueio > DateTime.Now)
            {
                ModelState.AddModelError("Login", $@"Número de tentativas excedido, Login bloqueado até {Convert.ToDateTime(usuario.DataDesbloqueio).ToLongTimeString()}");
                return View(usuarioLogin);
            }

            if (usuario.StatusUsuarioId == 2)
            {
                ModelState.AddModelError("Login", $@"Usuário Inativo");
            }
            var ultimoPagamento = db.Pagamentos.OrderByDescending(p => p.DataVencimento).FirstOrDefault(p => p.UsuarioId == usuario.UsuarioId);
            var perfil = db.UsuarioPerfis.Find(usuario.UsuarioId);
            if (ultimoPagamento != null && ultimoPagamento.DataVencimento.AddDays(15) <= DateTime.Now && perfil.PerfilId == 1)
            {
                ModelState.AddModelError("Login", $"Usuario bloqueado por pendência financeira da data de vencimento {ultimoPagamento.DataVencimento.ToShortDateString()}");
                return View(usuarioLogin);
            }

            //Não exite pagamento, apenas add 15 dias a data de cadastro
            if (ultimoPagamento == null && usuario.DataCadastro.AddDays(15) <= DateTime.Now && perfil.PerfilId == 1)
            {
                ModelState.AddModelError("Login", $"Usuario bloqueado por pendência financeira do cadastro");
                return View(usuarioLogin);
            }

            if (usuario.Senha.Equals(HashService.Crip(usuarioLogin.Senha)) && !usuario.UsuarioBloqueado() && usuario.StatusUsuarioId == 1)
            {
                var perfis = "";
                foreach (var usuarioPerfil in db.UsuarioPerfis.Where(u => u.UsuarioId == usuario.UsuarioId))
                {
                    using (var dbContext = new Context())
                        perfis += dbContext.Perfis.Find(usuarioPerfil.PerfilId)?.Descricao + ",";
                }
                FormsAuthentication.SetAuthCookie(usuario.Login, false);
                // Foi utilizado o DateTime.Now, pois a verificação do ticket é feita pelo horario do servidor
                var ticket = new FormsAuthenticationTicket(1, usuario.Login, DateTime.Now,
                    DateTime.Now.AddMinutes(30), false, perfis);
                var hash = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);
                if (ticket.IsPersistent)
                    cookie.Expires = ticket.Expiration;
                Response.Cookies.Add(cookie);
                usuario.RecuperaSenha = null;
                usuario.DataDesbloqueio = null;
                usuario.QuantTentativas = 0;
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction("Index", "Administrativa");
                var decodedUrl = Server.UrlDecode(returnUrl);
                if (Url.IsLocalUrl(decodedUrl))
                    return Redirect(decodedUrl);
                return RedirectToAction("Index", "Administrativa");
            }

            ModelState.AddModelError("Login", "Senha inválida");

            usuario.QuantTentativas++;
            if (usuario.QuantTentativas % 5 == 0)
            {
                usuario.DataDesbloqueio = DateTime.Now.AddMinutes(5);
                if (usuario.QuantTentativas % 10 == 0)
                {
                    var guid = Guid.NewGuid();
                    usuario.RecuperaSenha = guid;

                    var callbackUrl = Url.Action(
                        "RecuperarSenha", "Usuario",
                        new { recuperarSenha = usuario.RecuperaSenha },
                        protocol: Request.Url.Scheme);
                    EmailService.EnviarEmail(usuario.Email, "Segurança CEEM",
                        "Mantenha sua conta segura! Foi indentificada varias tentativas incorretas de login na sua conta, caso esteja tendo dificuldades para acessar a conta vc pode recuperar sua senha cliando neste " +
                        "<a href = \"" + callbackUrl + "\" >link </a>" +
                        ", se não foi você que tentou acessar sua conta, verifique a possibilidade de alterar sua senha");
                }
            }
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            return View(usuarioLogin);
        }

        public ActionResult Sair()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}