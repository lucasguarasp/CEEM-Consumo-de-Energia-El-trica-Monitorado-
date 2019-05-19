using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.ViewModels
{
    [NotMapped]
    public class CadastroViewModel:PreCadastroViewModel
    {
        [Required]
        public string NomeUsuario { get; set; }

        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Compare("Senha")]
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }

    }
}