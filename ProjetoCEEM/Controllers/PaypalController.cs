using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using ProjetoCEEM.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using ProjetoCEEM.Services;
using PayPal.Api;

namespace ProjetoCEEM.Controllers
{
    [Authorize]
    public class PaypalController : Controller
    {
        Context db = new Context();
        string pag;
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HistoricoPagamento()
        {
            var usuarioLogado = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var pagamento = db.Pagamentos.Where(p => p.UsuarioId == usuarioLogado.UsuarioId).ToList();
            return View(pagamento);
        }

        public ActionResult FailureView()
        {
            return View();
        }

        public ActionResult SuccessView()
        {
            return View();
        }

        public ActionResult PaymentWithPaypal()
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + "/Paypal/PaymentWithPayPal?";

                    var guid = Convert.ToString((new Random()).Next(100000));

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return RedirectToAction("HistoricoPagamento", "Paypal");
                    }
                }
            }

            catch (Exception)
            {
                return RedirectToAction("HistoricoPagamento", "Paypal");
            }

            var usuarioLogado = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            var pagamento = new Pagamento { DataPagamento = DateTime.Now, CompraId = pag, UsuarioId = usuarioLogado.UsuarioId, DataVencimento = DateTime.Now.AddDays(30) };
            db.Pagamentos.Add(pagamento);
            db.SaveChanges();

            return RedirectToAction("HistoricoPagamento", "Paypal");
        }

        private Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            payment = new Payment() { id = paymentId };
            pag = paymentId;

            return payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var itemList = new ItemList() { items = new List<Item>() };

            itemList.items.Add(new Item()
            {
                name = "Arduino",
                currency = "BRL",
                price = "80",
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = "80"
            };

            var amount = new Amount()
            {
                currency = "BRL",
                total = "80",
                details = details
            };

            var transactionList = new List<Transaction>
            {
                new Transaction()
                {
                    description = "Transaction description.",
                    invoice_number = "your invoice number",
                    amount = amount,
                    item_list = itemList
                }
            };

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return this.payment.Create(apiContext);
        }
    }
}