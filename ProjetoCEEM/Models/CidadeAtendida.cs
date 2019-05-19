using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetoCEEM.Models
{
    public class CidadeAtendida
    {
        public int CidadeAtendidaId { get; set; }
        public string Uf { get; set; }
        public string Cidade { get; set; }

        public static bool AtendeCidade(Context db, string cidade, string uf)
        {
            return db.CidadeAtendidas.Count(c => c.Cidade.ToLower().Equals(cidade.ToLower())&&c.Uf.ToLower().Equals(uf.ToLower())) > 0;
        }
        public static bool AtendeUf(Context db, string uf)
        {
            return db.CidadeAtendidas.Count(c => c.Uf.ToLower().Equals(uf.ToLower())) > 0;
        }
    }
}