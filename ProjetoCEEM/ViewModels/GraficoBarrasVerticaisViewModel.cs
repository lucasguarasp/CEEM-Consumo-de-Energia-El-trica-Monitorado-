using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjetoCEEM.Models;
namespace ProjetoCEEM.ViewModels
{
    public class GraficoBarrasVerticaisViewModel
    {
        public int EquipamentoId { get; set; }
        public string LocalEquipamento { get; set; }

        public int SensorId { get; set; }
        public string LocalSensor { get; set; }

        public int Ano { get; set; }
        public List<double> KwAnual { get; set; }
    }
}