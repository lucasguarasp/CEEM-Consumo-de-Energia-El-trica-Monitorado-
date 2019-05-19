using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.ViewModels
{
    [NotMapped]
    public class PrivacidadeSenhaViewModel
    {
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string NovaSenha { get; set; }

        [Compare("NovaSenha")]
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmarNovaSenha { get; set; }
    }
}