using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjetoCEEM.Models;

namespace ProjetoCEEM.ViewModels
{
    [NotMapped]
    public class PrivacidadePerguntaSegurancaViewModel
    {
        [Required]
        [MinLength(4)]
        public string Resposta { get; set; }
        [Required]
        public int PerguntaSegurancaId { get; set; }

        public SelectList PerguntaSegurancaList { get; set; }
    }
}