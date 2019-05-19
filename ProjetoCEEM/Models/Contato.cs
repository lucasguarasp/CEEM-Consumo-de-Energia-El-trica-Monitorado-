using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Contato
    {
        [Key]
        public int ContatoId { get; set; }

        public string Descricao { get; set; }

        // Chaves Estrangeiras
        public int CadastroId { get; set; }
        public int TipoContatoId { get; set; }

        // Metodos Virtuais
        public virtual Cadastro Cadastro { get; set; }
        public virtual TipoContato TipoContato { get; set; }

    }
}