using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ProjetoCEEM.Models;

namespace ProjetoCEEM.ViewModels
{
    [NotMapped]
    public class UsuarioEsqueciSenhaViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public Usuario RecuperarUsuarioEmail(Context db)
        {
            return db.Usuarios.Count(u => u.Email.Equals(Email)) > 0 ? db.Usuarios.Single(u => u.Email.Equals(Email)) : null;
        }
    }
}