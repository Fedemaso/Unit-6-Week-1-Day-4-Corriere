using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Corriere.Models
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string CodiceFiscale { get; set; }
        public string PartitaIVA { get; set; }
    }
}