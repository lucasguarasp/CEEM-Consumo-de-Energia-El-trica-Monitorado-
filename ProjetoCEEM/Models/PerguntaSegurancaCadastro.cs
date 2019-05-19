using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class PerguntaSegurancaCadastro
    {

        [Key]
        public int PerguntaSegurancaCadastroId { get; set; }
        
        public int PerguntaSeguracaId { get; set; }

        [Required]
        [MinLength(4)]
        public string Resposta { get; set; }

        [ForeignKey("PerguntaSeguracaId")]
        public virtual PerguntaSeguranca PerguntaSeguranca { get; set; }
    }
}