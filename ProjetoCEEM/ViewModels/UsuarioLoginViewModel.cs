using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjetoCEEM.Models;
using System.Linq;
using System.Web;
using ProjetoCEEM.Services;

namespace ProjetoCEEM.ViewModels
{
    [NotMapped]
    public class UsuarioLoginViewModel
    {
        [Required]
        [MinLength(4)]
        public string Login { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(30)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public Usuario Entrar(Context db)
        {
            return db.Usuarios.Count(u => u.Email.Equals(Login) || u.Login.Equals(Login)) > 0 ? db.Usuarios.Single(u => u.Email.Equals(Login) || u.Login.Equals(Login)) : null;
        }
    }
}