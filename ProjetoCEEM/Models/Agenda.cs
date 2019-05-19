using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Agenda
    {
        [Key]
        public int AgendaId { get; set; }

        public string Cliente { get; set; }

        public string Description { get; set; }

        public DateTime Start { get; set; }

        public string ThemeColor { get; set; }

        public int OrdemServicoId { get; set; }
        public virtual OrdemServico OrdemServico { get; set; }

        [EnumDataType(typeof(Tipo))]
        public Tipo TipoAgendamento { get; set; }

        public enum Tipo
        {
            [Display(Name = "Pré-Cadastro")]
            PreCadastro = 1,

            [Display(Name = "Manutenção")]
            Manuntecao = 2
        }
        public string GetTipoAgendamento {
            get
            {
                return Enum.GetName(typeof(Tipo), TipoAgendamento);
            }
        }
    }
}