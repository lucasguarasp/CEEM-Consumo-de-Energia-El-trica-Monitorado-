using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoCEEM.Models
{
    // This class is used to hold data when
    // communicating with the QnA Service
    public class QnAQuery
    {
        [JsonProperty(PropertyName = "question")]
        public string Pergunta { get; set; }

        [JsonProperty(PropertyName = "answer")]
        public string Resposta { get; set; }

        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }
}