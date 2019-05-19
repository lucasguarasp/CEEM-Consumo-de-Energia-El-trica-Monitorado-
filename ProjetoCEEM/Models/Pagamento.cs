using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Pagamento
    {
        [Key]
        public int PagamentoId { get; set; }
               
        public string CompraId { get; set; }

        public DateTime DataPagamento { get; set; }

        public DateTime DataVencimento { get; set; }

        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }

    }
}