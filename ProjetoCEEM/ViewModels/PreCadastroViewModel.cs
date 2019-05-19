using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ProjetoCEEM.Models;

namespace ProjetoCEEM.ViewModels
{
    [NotMapped]
    public class PreCadastroViewModel
    {
        [Required(ErrorMessage ="Digite seu nome")]
        [DisplayName("Nome Completo")]
        public string NomeCompleto { get; set; }
    
        [EmailAddress]
        [Required(ErrorMessage = "Digite um email válido")]
        public string Email { get; set; }

        [MinLength(15)]
        public string Celular { get; set; }
        [MinLength(14)]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Digite seu CEP")]
        [DisplayName("CEP")]
        [MinLength(9)]
        public string Cep { get; set; }
        [Required]
        [MinLength(3)]
        public string Rua { get; set; }
        [Required]
        [MinLength(3)]
        public string Bairro { get; set; }
        [Required]
        [MinLength(3)]
        public string Cidade { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(2)]
        [DisplayName("UF")]
        public string Uf { get; set; }

        [Required(ErrorMessage = "Digite o número de sua residência")]
        [DisplayName("Número")]
        public string Numero { get; set; }
        public string Complemento { get; set; }

        [Required(ErrorMessage = "Digite a data de nascimento")]
        [DisplayName("Data de Nascimento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; }


        [Required(ErrorMessage = "Digite uma resposta")]
        [MinLength(4)]
        [MaxLength(20)]
        public string Resposta { get; set; }
        [Required]
        [DisplayName("Pergunta")]
        public int PerguntaSegurancaId { get; set; }

        public virtual PerguntaSeguranca PerguntaSeguranca { get; set; }
    }
}