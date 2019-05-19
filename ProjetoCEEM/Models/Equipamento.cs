using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Equipamento
    {
        [Key]
        public int EquipamentoId { get; set; }

        public string Local { get; set; }
        public DateTime DataCadastro { get; set; }
        public int QuantMaxSensores { get; set; }

        // Chaves Estrangeiras
        public int UsuarioId { get; set; }

        // Metodos Virtuais
        public virtual Usuario Usuario { get; set; }

        public int QuantSensoresUsados(Context db)
        {
            return 0;
        }
    }
}