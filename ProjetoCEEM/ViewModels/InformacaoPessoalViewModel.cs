using ProjetoCEEM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.ViewModels
{
    [NotMapped]
    public class InformacaoPessoalViewModel
    {

        [Required]
        [MinLength(4)]
        public string NomeCompleto { get; set; }

        [Required]
        [DisplayName("Data de Nascimento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; }

        public Contato Contato { get; set; }

        public Endereco Endereco { get; set; }

    }
}