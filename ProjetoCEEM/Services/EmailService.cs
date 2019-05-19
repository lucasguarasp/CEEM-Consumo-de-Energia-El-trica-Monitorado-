using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace ProjetoCEEM.Services
{
    public class EmailService
    {
        public static string Msg { get; set; }
        public static string MsgError { get; set; }


        public static bool Chamado(string emailRemetente, string corpomsg, string chamado)
        {

            try
            {
                //Cria o endereço de email do remetente
                var de = new MailAddress(emailRemetente);
                //Cria o endereço de email do destinatário -->
                var para = new MailAddress("Projeto CEEM <ceem.projeto@gmail.com>");
                var mensagem = new MailMessage(de, para)
                {
                    IsBodyHtml = true,
                    //Assunto do email
                    Subject = "Chamado nº " + chamado,
                    //Conteúdo do email
                    Body = corpomsg + "<br /><br /><br /> <b>Remetente:</b> " + emailRemetente + "<br /> <b>Chamado nº: </b> " + chamado,
                    //Prioridade E-mail
                    Priority = MailPriority.Normal
                };
                //Cria o objeto que envia o e-mail
                var cliente = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("ceem.projeto@gmail.com", "Ceem2019**")
                };
                //Envia o email
                cliente.Send(mensagem);
                Msg = "E-mail enviado com sucesso";
                return true;
            }
            catch (Exception e)
            {
                Msg = "Erro ao enviar e-mail";
                MsgError = e.Message;
                return false;
            }
        }

        public static bool EntreEmContato(string emailRemetente, string nome, string corpomsg)
        {

            try
            {
                //Cria o endereço de email do remetente
                var de = new MailAddress(emailRemetente);
                //Cria o endereço de email do destinatário -->
                var para = new MailAddress("Projeto CEEM <ceem.projeto@gmail.com>");
                var mensagem = new MailMessage(de, para)
                {
                    IsBodyHtml = true,
                    //Assunto do email
                    Subject = "Entre em contato CEEM",
                    //Conteúdo do email
                    Body = corpomsg+ "<br /><br /><br /> <b>Remetente:</b> "+emailRemetente+"<br /> <b>Nome Remetente:</b> "+nome,
                    //Prioridade E-mail
                    Priority = MailPriority.Normal
                };
                //Cria o objeto que envia o e-mail
                var cliente = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("ceem.projeto@gmail.com", "Ceem2019**")
                };
                //Envia o email
                cliente.Send(mensagem);
                Msg = "E-mail enviado com sucesso";
                return true;
            }
            catch(Exception e)
            {
                Msg = "Erro ao enviar e-mail";
                MsgError = e.Message; 
                return false;
            }
        }

        public static bool EnviarEmail(string emailDestinatario, string assunto, string
            corpomsg)
        {

            try
            {
                //Cria o endereço de email do remetente
                var de = new MailAddress("Projeto CEEM <ceem.projeto@gmail.com>");
                //Cria o endereço de email do destinatário -->
                var para = new MailAddress(emailDestinatario);
                var mensagem = new MailMessage(de, para)
                {
                    IsBodyHtml = true,
                    //Assunto do email
                    Subject = assunto,
                    //Conteúdo do email
                    Body = corpomsg,
                    //Prioridade E-mail
                    Priority = MailPriority.Normal
                };
                //Cria o objeto que envia o e-mail
                var cliente = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("ceem.projeto@gmail.com", "Ceem2019**")
                };
                //Envia o email
                cliente.Send(mensagem);
                Msg = "E-mail enviado com sucesso";
                return true;
            }
            catch(Exception e)
            {
                Msg = "Erro ao enviar e-mail";
                MsgError = e.Message; 
                return false;
            }
        }
        public static T GetSetting<T>(string key, T defaultValue = default(T)) where T : IConvertible  
        {  
            var val = ConfigurationManager.AppSettings[key] ?? "";  
            var result = defaultValue;
            if (string.IsNullOrEmpty(val)) return result;
            var typeDefault = default(T);  
            if (typeof(T) == typeof(string))  
            {  
                typeDefault = (T)(object)string.Empty;  
            }

            if (typeDefault != null) result = (T) Convert.ChangeType(val, typeDefault.GetTypeCode());
            return result;  
        } 
    }
}