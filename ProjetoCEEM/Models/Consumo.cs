using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Consumo
    {
        [Key]
        public int ConsumoId { get; set; }

        public DateTime DataMedida { get; set; }
        public double Kwh { get; set; }

        // Chaves Estrangeiras
        public int SensorId { get; set; }
        public int EquipamentoId { get; set; }

        // Metodos Virtuais
        public virtual Sensor Sensor { get; set; }
        public virtual Equipamento Equipamento { get; set; }
    }
}