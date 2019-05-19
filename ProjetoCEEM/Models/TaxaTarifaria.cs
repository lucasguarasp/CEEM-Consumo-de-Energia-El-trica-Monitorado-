using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class TaxaTarifaria
    {
        [Key]
        public int Id { get; set; }        

        public double Valor { get; set; }

        [Index(IsUnique = true)]
        public string Distribuidora { get; set; }

        public string Uf { get; set; }

    }
}