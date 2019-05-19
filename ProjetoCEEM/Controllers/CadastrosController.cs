using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ProjetoCEEM.Models;
using ProjetoCEEM.Services;
using ProjetoCEEM.ViewModels;
using Context = ProjetoCEEM.Models.Context;

namespace ProjetoCEEM.Controllers
{
    public class CadastrosController : Controller
    {
        Context db = new Context();
        // GET: Cadastros
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Cadastrar()
        {
            ViewBag.PerguntaSegurancaId = new SelectList(db.PerguntaSegurancas, "PerguntaSegurancaId", "Descricao");
            return View();
        }
        public ActionResult ConcluirCadastro(string CadastroId)
        {
            var cadastros = db.Cadastros.ToList();
            Cadastro cadastro = new Cadastro();

            foreach (var item in cadastros)
            {
                if (HashService.Crip(item.CadastroId.ToString()).Equals(CadastroId))
                {
                    cadastro = item;
                    break;
                }
            }

            if (db.Usuarios.Count(u => u.CadastroId == cadastro.CadastroId) > 0)
            {
                return RedirectToAction("Login", "Usuario");
            }
            var endereco = db.Enderecos.Find(cadastro.CadastroId);
            var email = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 1);
            var celular = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 3);
            var telefone = db.Contatos.Single(c => c.CadastroId == cadastro.CadastroId && c.TipoContatoId == 2);
            var respostaPerguntaSeguranca = db.PerguntaSegurancaCadastros.Find(cadastro.CadastroId);
            ViewBag.PerguntaSegurancaId = new SelectList(db.PerguntaSegurancas, "PerguntaSegurancaId", "Descricao");
            var cadastroViewModel = new CadastroViewModel
            {
                Bairro = endereco.Bairro,
                Celular = celular.Descricao,
                Cep = endereco.Cep,
                Cidade = endereco.Cidade,
                Complemento = endereco.Complemento,
                ConfirmarSenha = null,
                DataNascimento = cadastro.DataNascimento,
                Email = email.Descricao,
                NomeCompleto = cadastro.NomeCompleto,
                NomeUsuario = null,
                Numero = endereco.NumeroCasa,
                Resposta = respostaPerguntaSeguranca.Resposta,
                Rua = endereco.Rua,
                Senha = null,
                Telefone = telefone.Descricao,
                Uf = endereco.Uf,
                PerguntaSegurancaId = respostaPerguntaSeguranca.PerguntaSeguracaId,
            };
            TempData["CadastroId"] = cadastro.CadastroId;
            return View("Cadastrar", cadastroViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar([Bind(Exclude = "InputForcaSenha,forcaSenha")] CadastroViewModel cadastroViewModel, int InputForcaSenha)
        {
            cadastroViewModel.DataNascimento = Convert.ToDateTime(cadastroViewModel.DataNascimento);
            ViewBag.PerguntaSegurancaId = new SelectList(db.PerguntaSegurancas, "PerguntaSegurancaId", "Descricao", cadastroViewModel.PerguntaSegurancaId);
            if (InputForcaSenha < 60)
            {
                ModelState.AddModelError("Senha", "É necessario ter uma senha Forte");
            }
            if (!ModelState.IsValid)
            {
                return View(cadastroViewModel);
            }
            var valido = true;

            var cadastroId = TempData["CadastroId"] == null ? 0 : (int)TempData["CadastroId"];
            TempData["CadastroId"] = cadastroId;
            TempData.Keep("CadastroId");

            if (!string.IsNullOrEmpty(cadastroViewModel.Telefone))
            {
                cadastroViewModel.Telefone = ApenasNumeros(cadastroViewModel.Telefone);
                if (cadastroViewModel.Telefone.Length == 10)
                {
                    Contato telefone;
                    if (db.Contatos.Count(c => c.CadastroId == cadastroId && c.TipoContatoId == 2) > 0)
                    {
                        telefone = db.Contatos.Single(c => c.CadastroId == cadastroId && c.TipoContatoId == 2);
                        db.Entry(telefone).State = EntityState.Modified;
                    }
                    else
                    {
                        telefone = new Contato { Descricao = "", TipoContatoId = 2, CadastroId = cadastroId };
                        db.Contatos.Add(telefone);
                    }
                }
                else
                {
                    ModelState.AddModelError("Telefone", "Formato do telefone inválido");
                    valido = false;
                }
            }
            else
            {
                Contato telefone;
                if (db.Contatos.Count(c => c.CadastroId == cadastroId && c.TipoContatoId == 2) > 0)
                {
                    telefone = db.Contatos.Single(c => c.CadastroId == cadastroId && c.TipoContatoId == 2);
                    db.Entry(telefone).State = EntityState.Modified;
                }
                else
                {
                    telefone = new Contato { Descricao = "", TipoContatoId = 2, CadastroId = cadastroId };
                    db.Contatos.Add(telefone);
                }
            }
            if (!CidadeAtendida.AtendeCidade(db, cadastroViewModel.Cidade, cadastroViewModel.Uf))
            {
                ModelState.AddModelError("Cidade", "Atualmente não atendemos a cidade informada");
                valido = false;
            }
            if (!CidadeAtendida.AtendeUf(db, cadastroViewModel.Uf))
            {
                ModelState.AddModelError("Uf", "Atualmente não atendemos a região informada");
                valido = false;
            }
            if (DateTime.Compare(cadastroViewModel.DataNascimento, DateTime.Now.AddYears(-18)) >= 0)
            {
                ModelState.AddModelError("DataNascimento", "É necessario ter mais de 18 anos para se cadastrar");
                valido = false;
            }

            if (!string.IsNullOrEmpty(cadastroViewModel.Celular))
            {
                cadastroViewModel.Celular = ApenasNumeros(cadastroViewModel.Celular);
                if (cadastroViewModel.Celular.Length == 11)
                {
                    Contato celular;
                    if (db.Contatos.Count(c => c.CadastroId == cadastroId && c.TipoContatoId == 2) > 0)
                    {
                        celular = db.Contatos.Single(c => c.CadastroId == cadastroId && c.TipoContatoId == 2);
                        db.Entry(celular).State = EntityState.Modified;
                    }
                    else
                    {
                        celular = new Contato { Descricao = "", TipoContatoId = 3, CadastroId = cadastroId };
                        db.Contatos.Add(celular);
                    }
                }
                else
                {
                    ModelState.AddModelError("Celular", "Formato do celular inválido");
                    valido = false;
                }
            }
            else
            {
                Contato celular;
                if (db.Contatos.Count(c => c.CadastroId == cadastroId && c.TipoContatoId == 2) > 0)
                {
                    celular = db.Contatos.Single(c => c.CadastroId == cadastroId && c.TipoContatoId == 2);
                    db.Entry(celular).State = EntityState.Modified;
                }
                else
                {
                    celular = new Contato { Descricao = "", TipoContatoId = 3, CadastroId = cadastroId };
                    db.Contatos.Add(celular);
                }
            }

            var usuario = new Usuario
            {
                CadastroId = cadastroId,
                DataCadastro = DateTime.Now,
                Email = cadastroViewModel.Email,
                Login = cadastroViewModel.NomeUsuario,
                Senha = HashService.Crip(cadastroViewModel.Senha),
                StatusUsuarioId = 2,
                UsuarioId = 0
            };
            if (!usuario.EmailDisponivel(db))
            {
                ModelState.AddModelError("Email", "Este email já está sendo usado");
                valido = false;
            }
            if (!usuario.NomeUsuarioDisponivel(db))
            {
                ModelState.AddModelError("NomeUsuario", "Este nome de usuário já está sendo usado");
                valido = false;
            }
            if (!usuario.NomeUsuarioValido())
            {
                ModelState.AddModelError("NomeUsuario", "Nome de usuário inválido");
                valido = false;
            }

            if (!valido) return View(cadastroViewModel);
            Cadastro preCadastro;
            if (db.Cadastros.Count(c => c.CadastroId == cadastroId) > 0)
            {
                preCadastro = db.Cadastros.Find(cadastroId);
                preCadastro.NomeCompleto = cadastroViewModel.NomeCompleto;
                preCadastro.StatusCadastroId = 2;
                preCadastro.DataNascimento = cadastroViewModel.DataNascimento;
                db.Entry(preCadastro).State = EntityState.Modified;
            }
            else
            {
                preCadastro = new Cadastro { DataCriacao = DateTime.Now, NomeCompleto = cadastroViewModel.NomeCompleto, StatusCadastroId = 2, DataNascimento = cadastroViewModel.DataNascimento, PerguntaSegurancaCadastroId = 0 };
                db.Cadastros.Add(preCadastro);

                var email = new Contato { Descricao = cadastroViewModel.Email.ToLower(), TipoContatoId = 1, CadastroId = 0 };

                var perguntaSegurancaCadastro = new PerguntaSegurancaCadastro
                {
                    PerguntaSeguracaId = cadastroViewModel.PerguntaSegurancaId,
                    Resposta = cadastroViewModel.Resposta
                };
                var endereco = new Endereco
                {
                    Bairro = cadastroViewModel.Bairro,
                    Cidade = cadastroViewModel.Cidade,
                    Cep = ApenasNumeros(cadastroViewModel.Cep),
                    NumeroCasa = cadastroViewModel.Numero,
                    Rua = cadastroViewModel.Rua,
                    Uf = cadastroViewModel.Uf,
                    CadastroId = preCadastro.CadastroId,
                    Complemento = cadastroViewModel.Complemento
                };


                db.Contatos.Add(email);
                db.Enderecos.Add(endereco);
                db.PerguntaSegurancaCadastros.Add(perguntaSegurancaCadastro);
            }
            var usuarioPerfil = new UsuarioPerfil
            {
                UsuarioId = 0,
                PerfilId = 1
            };
            db.Usuarios.Add(usuario);
            db.UsuarioPerfis.Add(usuarioPerfil);
            CadastraEquipamento(usuario.UsuarioId, DateTime.Now, 4, "Default");
            db.SaveChanges();
            if (!EmailService.GetSetting<bool>("EnviarEmail")) return RedirectToAction("Index", "Home");
            var emailEnviado = EmailService.EnviarEmail(cadastroViewModel.Email, "Cadastro no Projeto CEEM",
                "Parabéns e obrigado por se cadastrar no Projeto CEEM."
            );

            if (ModelState.IsValid)
            {
                TempData["message"] = "Cadastro realizado";
                // if (emailEnviado)
                //return RedirectToAction("Index", "Cadastros");
            }
            TempData.Remove("CadastroId");
            //ModelState.AddModelError("Email", string.Concat(EmailService.Msg, ", Erro: ", EmailService.MsgError));
            return View(cadastroViewModel);
        }



        public ActionResult PreCadastrar()
        {

            ViewBag.PerguntaSegurancaId = new SelectList(db.PerguntaSegurancas, "PerguntaSegurancaId", "Descricao");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreCadastrar(PreCadastroViewModel preCadastroViewModel)
        {
            preCadastroViewModel.DataNascimento = Convert.ToDateTime(preCadastroViewModel.DataNascimento);
            ViewBag.PerguntaSegurancaId = new SelectList(db.PerguntaSegurancas, "PerguntaSegurancaId", "Descricao", preCadastroViewModel.PerguntaSegurancaId);
            if (!ModelState.IsValid) return View(preCadastroViewModel);
            var valido = true;
            if (!string.IsNullOrEmpty(preCadastroViewModel.Telefone))
            {
                preCadastroViewModel.Telefone = ApenasNumeros(preCadastroViewModel.Telefone);
                if (preCadastroViewModel.Telefone.Length == 10)
                {
                    var telefone = new Contato { Descricao = ApenasNumeros(preCadastroViewModel.Telefone), TipoContatoId = 2, CadastroId = 0 };
                    db.Contatos.Add(telefone);
                }
                else
                {
                    ModelState.AddModelError("Telefone", "Formato do telefone inválido");
                    valido = false;
                }
            }
            else
            {
                var telefone = new Contato { Descricao = "", TipoContatoId = 2, CadastroId = 0 };
                db.Contatos.Add(telefone);
            }
            if (!CidadeAtendida.AtendeCidade(db, preCadastroViewModel.Cidade, preCadastroViewModel.Uf))
            {
                ModelState.AddModelError("Cidade", "Atualmente não atendemos a cidade informada");
                valido = false;
            }
            if (!CidadeAtendida.AtendeUf(db, preCadastroViewModel.Uf))
            {
                ModelState.AddModelError("Uf", "Atualmente não atendemos a região informada");
                valido = false;
            }
            if (DateTime.Compare(preCadastroViewModel.DataNascimento, DateTime.Now.AddYears(-18)) >= 0)
            {
                ModelState.AddModelError("DataNascimento", "É necessario ter mais de 18 anos para se cadastrar");
                valido = false;
            }

            if (!string.IsNullOrEmpty(preCadastroViewModel.Celular))
            {
                preCadastroViewModel.Celular = ApenasNumeros(preCadastroViewModel.Celular);
                if (preCadastroViewModel.Celular.Length == 11)
                {
                    var celular = new Contato { Descricao = ApenasNumeros(preCadastroViewModel.Celular), TipoContatoId = 3, CadastroId = 0 };
                    db.Contatos.Add(celular);
                }
                else
                {
                    ModelState.AddModelError("Celular", "Formato do celular inválido");
                    valido = false;
                }
            }
            else
            {
                var celular = new Contato { Descricao = "", TipoContatoId = 3, CadastroId = 0 };
                db.Contatos.Add(celular);
            }

            if (!valido) return View(preCadastroViewModel);
            var preCadastro = new Cadastro { DataCriacao = DateTime.Now, NomeCompleto = preCadastroViewModel.NomeCompleto, StatusCadastroId = 1, DataNascimento = preCadastroViewModel.DataNascimento, PerguntaSegurancaCadastroId = 0 };
            db.Cadastros.Add(preCadastro);
            var chamado = preCadastro.CadastroId + "" + (DateTime.Now).ToString().Replace("/", "").Replace(" ", "").Replace(":", "");
            var ordemServico = new OrdemServico { DataCriacao = preCadastro.DataCriacao, CadastroId = preCadastro.CadastroId, StatusOrdemServicoId = 1, NumeroOS = chamado};
            var email = new Contato { Descricao = preCadastroViewModel.Email.ToLower(), TipoContatoId = 1, CadastroId = 0 };

            var perguntaSegurancaCadastro = new PerguntaSegurancaCadastro
            {
                PerguntaSeguracaId = preCadastroViewModel.PerguntaSegurancaId,
                Resposta = preCadastroViewModel.Resposta
            };
            var endereco = new Endereco
            {
                Bairro = preCadastroViewModel.Bairro,
                Cidade = preCadastroViewModel.Cidade,
                Cep = ApenasNumeros(preCadastroViewModel.Cep),
                NumeroCasa = preCadastroViewModel.Numero,
                Rua = preCadastroViewModel.Rua,
                Uf = preCadastroViewModel.Uf,
                CadastroId = preCadastro.CadastroId,
                Complemento = preCadastroViewModel.Complemento
            };
            db.OrdemServicos.Add(ordemServico);
            db.Contatos.Add(email);
            db.Enderecos.Add(endereco);
            db.PerguntaSegurancaCadastros.Add(perguntaSegurancaCadastro);
            db.SaveChanges();

            if (!EmailService.GetSetting<bool>("EnviarEmail")) return RedirectToAction("Index", "Home");
            var emailEnviado = EmailService.EnviarEmail(preCadastroViewModel.Email, "Pré-Cadastro no Projeto CEEM",
                "Seu Pré-cadastro foi realizado com sucesso, aguarde o retorno."
            );

            if (ModelState.IsValid)
            {
                TempData["message"] = "Pré-Cadastro realizado";
                // if (emailEnviado)
                //return RedirectToAction("Index", "Cadastros");
            }

            return View(preCadastroViewModel);
        }
        public static string ApenasNumeros(string str)
        {
            var apenasDigitos = new Regex(@"[^\d]");
            return apenasDigitos.Replace(str, string.Empty);
        }

        public void CadastraEquipamento(int usuarioId, DateTime dataCadastro, int quantMaxSensores, string localInstalacao)
        {
            var equipamento = new Equipamento
            {
                UsuarioId = usuarioId,
                DataCadastro = dataCadastro,
                QuantMaxSensores = quantMaxSensores,
                Local = localInstalacao,
                EquipamentoId = 0
            };
            db.Equipamentos.Add(equipamento);
            for (int i = 1; i <= equipamento.QuantMaxSensores; i++)
            {
                var sensor = new Sensor
                {
                    Local = "Sensor " + i,
                    EquipamentoId = equipamento.EquipamentoId,
                    SensorId = i
                };
                db.Sensores.Add(sensor);
            }

        }
    }
}