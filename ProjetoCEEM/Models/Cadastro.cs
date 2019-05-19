using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations.Model;
using System.Web.Mvc;

namespace ProjetoCEEM.Models
{
    public class Cadastro
    {
        [Key]
        public int CadastroId { get; set; }
        [Required]
        [MinLength(4)]
        public string NomeCompleto { get; set; }
        [Required]
        public DateTime DataCriacao { get; set; }
        [Required]
        [Column(TypeName="datetime2")]
        public DateTime DataNascimento { get; set; }
        [Required]
        public int StatusCadastroId { get; set; }
        [Required]
        public int PerguntaSegurancaCadastroId { get; set; }


        public virtual StatusCadastro StatusCadastro { get; set; }
        public virtual PerguntaSegurancaCadastro PerguntaSegurancaCadastro { get; set; }

       
    }
}