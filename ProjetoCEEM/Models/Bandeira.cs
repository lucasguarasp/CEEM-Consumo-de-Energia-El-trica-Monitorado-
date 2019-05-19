using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class Bandeira
    {
        [Key]
        public int BandeiraId { get; set; }

        [Index(IsUnique = true)]
        public string Cor { get; set; }

        public double Valor { get; set; }

        public string Descricao { get; set; }

        public bool BandeiraVigente { get; set; }

    }
}