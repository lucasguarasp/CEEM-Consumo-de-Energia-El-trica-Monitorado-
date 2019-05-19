using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ProjetoCEEM.Services;

namespace ProjetoCEEM.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EntreEmContato(string nome, string email, string mensagem)
        {
            EmailService.EntreEmContato(email, nome, mensagem);

            if (ModelState.IsValid)
            {
                TempData["message"] = "Email enviado!!!";               
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Desenvolvedor")]
        public ActionResult Exception()
        {
            ViewBag.Message = "Your contact page.";
            throw new Exception("teste");
        }
    }
}
