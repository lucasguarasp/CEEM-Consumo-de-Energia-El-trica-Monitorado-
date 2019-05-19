using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.ViewModels
{
    public class GraficoPizzaViewModel
    {
        public int EquipamentoId { get; set; }
        public string LocalEquipamento { get; set; }

        public int SensorId { get; set; }
        public string LocalSensor { get; set; }

        public int Ano { get; set; }
        public int Mes { get; set; }
        public double PorcentagemKW { get; set; }
    }
}