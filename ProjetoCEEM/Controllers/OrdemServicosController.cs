using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoCEEM.Models;

namespace ProjetoCEEM.Controllers
{
    [Authorize]
    public class OrdemServicosController : Controller
    {
        Context db = new Context();

        // GET: OrdemServicos
        public ActionResult Index()
        {
            var OrdemServicos = db.OrdemServicos.ToList();

            if (OrdemServicos != null)
            {
                return View(OrdemServicos);
            }

            return View();
        }
        public ActionResult MinhasOS()
        {
            var usuario = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));

            var OrdemServicos = db.OrdemServicos.Where(os => os.Cadastro.CadastroId == usuario.Cadastro.CadastroId).ToList();

            if (OrdemServicos != null)
            {
                return View(OrdemServicos);
            }

            return View();
        }
    }
}