using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class OrdemServico
    {
        [Key]
        public int OrdemServicoId { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string NumeroOS { get; set; }

        // Chaves Estrangeiras
        public int? UsuarioCriacaoId { get; set; }
        public int? UsuarioAtendenteId { get; set; }
        public int? CadastroId { get; set; }
        public int StatusOrdemServicoId { get; set; }

        // Metodos Virtuais
        [ForeignKey("UsuarioCriacaoId")]
        public virtual Usuario Usuario { get; set; }
        [ForeignKey("UsuarioAtendenteId")]
        public virtual Usuario Tecnico { get; set; }
        public virtual Cadastro Cadastro { get; set; }
        public virtual StatusOrdemServico StatusOrdemServico { get; set; }
       
    }
}