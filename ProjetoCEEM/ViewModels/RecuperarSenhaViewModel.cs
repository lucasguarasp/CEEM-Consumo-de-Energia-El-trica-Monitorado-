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
    public class RecuperarSenhaViewModel
    {
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Compare("Senha")]
        [MinLength(8)]
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }

        [Required]
        [MinLength(5)]
        public string Resposta { get; set; }

        public string Pergunta { get; set; }
    }
}