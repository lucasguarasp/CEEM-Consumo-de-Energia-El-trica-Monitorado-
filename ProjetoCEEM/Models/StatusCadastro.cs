using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class StatusCadastro
    {
        [Key]
        public int StatusCadastroId { get; set; }
        [Required]
        public string Descricao { get; set; }
    }
}