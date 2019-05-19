using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoCEEM.Models;

namespace ProjetoCEEM.Controllers
{
    [Authorize]
    public class GerenciamentoController : Controller
    {
        Context db = new Context();

        // GET: Gerenciamento
        public ActionResult Index()
        {
            var tarifas = db.TaxaTarifarias.ToList();

            if (tarifas != null)
            {
                return View(tarifas);
            }

            return View();
        }

        public ActionResult AlterarBandeira(int id)
        {
            var bandeira = db.Bandeiras.Single(b => b.BandeiraVigente == true);
            bandeira.BandeiraVigente = false;

            var setNewBandeira = db.Bandeiras.Single(b => b.BandeiraId == id);
            setNewBandeira.BandeiraVigente = true;

            db.Entry(bandeira).State = System.Data.Entity.EntityState.Modified;
            db.Entry(setNewBandeira).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            if (ModelState.IsValid)
            {
                TempData["message"] = "Bandeira ativada com sucesso!";
            }            

            return RedirectToAction("Index");
        }

        public ActionResult getTarifas()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult EditarTaxas(string Codigo, string Valor)
        {
            var status = false;
            int cod = Convert.ToInt32(Codigo);
            var taxa = db.TaxaTarifarias.Where(c => c.Id == cod).FirstOrDefault();

            if (taxa != null)
            {
                var val = Convert.ToDouble(Valor);
                taxa.Valor = val;
                db.Entry(taxa).State = EntityState.Modified;
                status = true;
                db.SaveChanges();
            }
            return new JsonResult { Data = new { status = status } };
        }


        [HttpPost]
        public JsonResult AddTaxa(string distribuidora, string uf, string valor)
        {
            var status = false;

            if (!string.IsNullOrWhiteSpace(distribuidora) || !string.IsNullOrWhiteSpace(uf) || !string.IsNullOrWhiteSpace(valor))
            {
                var val = Convert.ToDouble(valor);
                var taxa = new TaxaTarifaria { Distribuidora = distribuidora, Uf = uf, Valor = val };
                db.TaxaTarifarias.Add(taxa);
                db.SaveChanges();

                status = true;
            }

            return new JsonResult { Data = new { status = status } };
        }


    }

}