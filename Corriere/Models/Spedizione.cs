using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Corriere.Models
{
    public class Spedizione
    {
        public int SpedizioneID { get; set; }
        public int ClienteID { get; set; }
        public string NumeroSpedizione { get; set; }
        public DateTime DataSpedizione { get; set; }
        public decimal Peso { get; set; }
        public string CittaDestinataria { get; set; }
        public string IndirizzoDestinatario { get; set; }
        public string NominativoDestinatario { get; set; }
        public decimal Costo { get; set; }
        public string NomeCliente { get; set; }
        public DateTime DataConsegnaPrevista { get; set; }
        public string Stato { get; set; }
        public string Posizione { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataAggiornamento { get; set; }
        public string CodiceFiscaleCliente { get; set; }
        public string DataSpedizioneFormatted => DataSpedizione.ToShortDateString();

        public string DataConsegnaPrevistaFormatted => DataConsegnaPrevista.ToShortDateString();


    }



    public class AggiornamentoSpedizione
    {
        public int Id { get; set; }
        public int NumeroSpedizione { get; set; }
        public string Stato { get; set; }
        public string Luogo { get; set; }
        public string Descrizione { get; set; }
        public DateTime DataOraAggiornamento { get; set; }
    }


    public class SpedizioneConCliente
    {
        public int SpedizioneID { get; set; }
        public string NumeroSpedizione { get; set; }
        public DateTime DataSpedizione { get; set; }
        public decimal Peso { get; set; }
        public string CittaDestinataria { get; set; }
        public string IndirizzoDestinatario { get; set; }
        public string NominativoDestinatario { get; set; }
        public decimal Costo { get; set; }
        public DateTime DataConsegnaPrevista { get; set; }
        public string NomeCliente { get; set; }

        public string Stato { get; set; }

        public string DataSpedizioneFormatted => DataSpedizione.ToShortDateString();

        public string DataConsegnaPrevistaFormatted => DataConsegnaPrevista.ToShortDateString();

    }


    public class SpedizioneInConsegna
    {
        public string NumeroSpedizione { get; set; }
        public DateTime DataSpedizione { get; set; }
        public string CittaDestinataria { get; set; }
        public string IndirizzoDestinatario { get; set; }
        public string NominativoDestinatario { get; set; }
        public string NomeCliente { get; set; }
        public string Stato { get; set; }

        public List<SpedizioneInConsegna> Spedizioni { get; set; }
    }




    public class SpedizioneInAttesa
    {
        public int SpedizioneID { get; set; }       
        public string NumeroSpedizione { get; set; }        
        public DateTime DataSpedizione { get; set; }        
        public string CittaDestinataria { get; set; }        
        public string Stato { get; set; }
    }


    public class SpedizioniPerCitta
    {
        public string CittaDestinataria { get; set; }
        public int NumeroSpedizioni { get; set; }
    }



}