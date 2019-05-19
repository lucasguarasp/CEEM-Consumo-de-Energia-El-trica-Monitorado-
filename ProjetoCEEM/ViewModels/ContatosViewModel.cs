using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.ViewModels
{
    public class ContatosViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Digite um email válido")]
        public string Email { get; set; }

        [MinLength(15)]
        public string Celular { get; set; }
        [MinLength(14)]
        public string Telefone { get; set; }
    }
}