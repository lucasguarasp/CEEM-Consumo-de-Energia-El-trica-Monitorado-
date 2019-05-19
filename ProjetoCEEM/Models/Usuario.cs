using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId {get;set;}
        [Index(IsUnique = true)]
        public string Login {get;set;}
        [Index(IsUnique = true)]
        public string Email {get;set;}
        public string Senha {get;set;}
        public DateTime DataCadastro {get;set;}
        public DateTime? DataInativacao {get;set;}
        public int QuantTentativas { get; set; }
        [Column(TypeName="datetime2")]
        public DateTime? DataDesbloqueio { get; set; }
        public Guid? RecuperaSenha { get; set; }
        public string NomeImagem { get; set; }

        // Chaves Estrangeiras
        public int StatusUsuarioId {get;set;}
        public int CadastroId {get;set;}

        // Metodos Virtuais
        public virtual Cadastro Cadastro { get; set; }
        public virtual StatusUsuario StatusUsuario { get; set; }

        public bool EmailDisponivel(Context db)
        {
            return db.Usuarios.Count(u =>u.Email.Equals(Email)) == 0;
        }

        public bool NomeUsuarioDisponivel(Context db)
        {
            return db.Usuarios.Count(u =>u.Login.Equals(Login)) == 0;
        }

        public bool NomeUsuarioValido()
        {
            return !(Login.Contains(" ") || Regex.IsMatch(Login, @"[^a-zA-Z0-9]"));;
        }

        public bool UsuarioBloqueado()
        {
            return DataDesbloqueio > DateTime.Now;
        }
    }
}