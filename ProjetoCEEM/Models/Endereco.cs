using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Endereco
    {
        [Key]
        public int EnderecoId { get; set; }

        public string Cep { get; set; }
        public string Rua { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string NumeroCasa { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string Complemento { get; set; }

        // Chaves Estrangeiras
        public int? CadastroId { get; set; }

        // Metodos Virtuais
        public Cadastro Cadastro { get; set; }
    }
}