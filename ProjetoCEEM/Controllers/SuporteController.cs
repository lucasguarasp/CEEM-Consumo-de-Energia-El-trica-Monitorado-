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

namespace ProjetoCEEM.Controllers
{
    [Authorize]
    public class SuporteController : Controller
    {
        Context db = new Context();


        public ActionResult Chamado()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Chamado(string emailRemetente, string mensagem, string chamado)
        {
            //pega usuario logado
            var usuarioLogado = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));

            //numero do chamado recebe Id do cliente + data atual 
            chamado = usuarioLogado.CadastroId + "" + (DateTime.Now).ToString().Replace("/", "").Replace(" ", "").Replace(":", "");
            emailRemetente = usuarioLogado.Email;

            //verifica se exite OS com id do usuario e se existe alguma OS aberta ou em andamento
            var os = db.OrdemServicos.Where(c => c.CadastroId == usuarioLogado.CadastroId && (c.StatusOrdemServicoId == 1 || c.StatusOrdemServicoId == 2)).Count();

            if (os == 0)
            {
                //cria nova OS
                var ordemServico = new OrdemServico { DataCriacao = DateTime.Now, CadastroId = usuarioLogado.CadastroId, StatusOrdemServicoId = 1, NumeroOS = chamado, UsuarioCriacaoId = usuarioLogado.CadastroId };
                db.OrdemServicos.Add(ordemServico);
                db.SaveChanges();
                EmailService.Chamado(emailRemetente, mensagem, chamado);

                TempData["titulo"] = "Sucesso";
                TempData["message"] = "Chamado de nº " + chamado + " criado, aguarde retorno";
                TempData["tipo"] = "success";

            }
            else
            {

                TempData["titulo"] = "Erro";
                TempData["message"] = "Já existe um chamado de nº " + db.OrdemServicos.Where(c => c.CadastroId == usuarioLogado.CadastroId && (c.StatusOrdemServicoId == 1 || c.StatusOrdemServicoId == 2)).FirstOrDefault().NumeroOS + ", em " + db.OrdemServicos.Where(c => c.CadastroId == usuarioLogado.CadastroId && (c.StatusOrdemServicoId == 1 || c.StatusOrdemServicoId == 2)).FirstOrDefault().StatusOrdemServico.Descricao + ", aguarde retorno";
                TempData["tipo"] = "error";
            }


            return RedirectToAction("Chamado");
        }


        public ActionResult EditarStatusOS(int Id, string NumeroOS, string StatusOS)
        {
            //pega usuario logado
            var usuarioLogado = db.Usuarios.Single(u => u.Login.Equals(User.Identity.Name));
            
            var os = db.OrdemServicos.Where(c => c.NumeroOS == NumeroOS && (c.StatusOrdemServicoId == 1 || c.StatusOrdemServicoId == 2)).FirstOrDefault();
            var contato = db.Contatos.Where(c => c.CadastroId == Id && c.TipoContatoId == 1).FirstOrDefault();

            if (os != null)
            {
                if (StatusOS == "2")
                {
                    //Ler (Andamento) na OS
                    os.StatusOrdemServicoId = 2;
                    os.UsuarioAtendenteId = usuarioLogado.Cadastro.CadastroId;
                    os.DataAlteracao = DateTime.Now;
                    db.Entry(os).State = System.Data.Entity.EntityState.Modified;

                    var emailEnviado = EmailService.EnviarEmail(contato.Descricao, "Chamado CEEM",
                    "Olá " + os.Cadastro.NomeCompleto + ", o chamado de nº: " + os.NumeroOS + ", está " + os.StatusOrdemServico.Descricao + " por: " + usuarioLogado.Cadastro.NomeCompleto + ", aguarde o retorno");

                    TempData["titulo"] = "Sucesso";
                    TempData["message"] = "Chamado de nº " + NumeroOS + " agora está " + os.StatusOrdemServico.Descricao;
                    TempData["tipo"] = "success";

                }

                if (StatusOS == "3" && os.StatusOrdemServicoId == 2)
                {
                    //Conclui OS
                    os.StatusOrdemServicoId = 3;
                    os.UsuarioAtendenteId = usuarioLogado.Cadastro.CadastroId;
                    os.DataAlteracao = DateTime.Now;
                    db.Entry(os).State = System.Data.Entity.EntityState.Modified;

                      var emailEnviado = EmailService.EnviarEmail(contato.Descricao, "Chamado CEEM",
                      "Olá " + os.Cadastro.NomeCompleto + ", o chamado de nº: " + os.NumeroOS + ", foi " + os.StatusOrdemServico.Descricao + " por: " + usuarioLogado.Cadastro.NomeCompleto);

                    TempData["titulo"] = "Sucesso";
                    TempData["message"] = "Chamado de nº " + NumeroOS + " agora foi " + os.StatusOrdemServico.Descricao;
                    TempData["tipo"] = "success";

                }

                if (ModelState.IsValid)
                {
                    TempData["message"] = "Ordem serviço " + os.StatusOrdemServico.Descricao.ToUpper();
                }

                db.SaveChanges();
            }
            return RedirectToAction("Index", "OrdemServicos");
        }


        private static string _OcpApimSubscriptionKey = "13900f0ac44a4b2798c01054a9a3cf24";
        private static string _KnowledgeBase = "340d58dd-0d3b-46d4-a34c-a3b2f68a4652";

        // GET: Suporte
        public async Task<ActionResult> Chat(string searchString)
        {
            QnAQuery objQnAResult = new QnAQuery();

            try
            {
                if (searchString != null)
                {
                    objQnAResult = await QueryQnABot(searchString);
                }
                return View(objQnAResult);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View(objQnAResult);
            }
        }
        private static async Task<QnAQuery> QueryQnABot(string Query)
        {
            QnAQuery QnAQueryResult = new QnAQuery();

            using (HttpClient client = new System.Net.Http.HttpClient())
            {
                string RequestURI = String.Format("{0}{1}{2}{3}{4}", @"https://westus.api.cognitive.microsoft.com/", @"qnamaker/v1.0/", @"knowledgebases/", _KnowledgeBase, @"/generateAnswer");

                var httpContent = new StringContent($"{{\"question\": \"{Query}\"}}", Encoding.UTF8, "application/json");

                httpContent.Headers.Add("Ocp-Apim-Subscription-Key", _OcpApimSubscriptionKey);

                System.Net.Http.HttpResponseMessage msg = await client.PostAsync(RequestURI, httpContent);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();

                    QnAQueryResult = JsonConvert.DeserializeObject<QnAQuery>(JsonDataResponse);

                    if (QnAQueryResult.Resposta.Equals("No good match found in the KB"))
                    {
                        QnAQueryResult.Resposta = "Não possuo esta informação. Por favor abra um chamado ou entre em contato conosco.";
                    }
                }
            }
            return QnAQueryResult;
        }
    }
}