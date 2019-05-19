using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoCEEM.Controllers
{
    public class ErrosController : Controller
    {
        // GET: Erros
        public ActionResult Erro404()
        {
            return View();
        }
    }
}