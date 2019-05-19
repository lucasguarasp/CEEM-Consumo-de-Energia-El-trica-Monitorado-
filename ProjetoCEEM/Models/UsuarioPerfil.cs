using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class UsuarioPerfil
    {
        [Key]
        public int UsuarioPerfilId { get; set; }
        [Index(IsUnique = true)]
        [Required]
        public int UsuarioId { get; set; }
        [Required]
        public int PerfilId { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual Perfil Perfil { get; set; }
    }
}