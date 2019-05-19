using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Sensor
    {
        [Key]
        [Column(Order = 2)]
        [Required]
        public int EquipamentoId { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        public int SensorId { get; set; }

        public string Local { get; set; }

        // Metodos Virtuais
        public virtual Equipamento Equipamento { get; set; }
        public virtual ICollection<Consumo> SensorConsumos { get; set; }
        public string Consumos()
        {
            string consumosString = "";
            foreach (var consumo in SensorConsumos)
            {
                consumosString = string.Concat(consumosString, consumo.Kwh);
            }
            return consumosString;
        }
    }
}