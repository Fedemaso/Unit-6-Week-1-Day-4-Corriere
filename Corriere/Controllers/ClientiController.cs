using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using Corriere.Models;

namespace Corriere.Controllers
{
    public class ClientiController : Controller
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        }


        public ActionResult Index()
        {
            var clienti = CorriereDB.GetClienti(); 
            return View(clienti);
        }



        public ActionResult NuovoCliente()
        {
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuovoCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    InserisciNuovoCliente(cliente);

                    TempData["Message"] = "Cliente aggiunto con successo.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Si è verificato un errore durante l'aggiunta del cliente: " + ex.Message;
                }
            }

            return View(cliente);
        }





        private void InserisciNuovoCliente(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"INSERT INTO Clienti (Nome, Cognome, CodiceFiscale, PartitaIVA)
                                 VALUES (@Nome, @Cognome, @CodiceFiscale, @PartitaIVA)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                    cmd.Parameters.AddWithValue("@Cognome", cliente.Cognome);
                    cmd.Parameters.AddWithValue("@CodiceFiscale", cliente.CodiceFiscale);
                    cmd.Parameters.AddWithValue("@PartitaIVA", cliente.PartitaIVA);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public ActionResult Dettagli(int id)
        {
            Cliente cliente = CorriereDB.GetClienteById(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }






        public ActionResult ModificaCliente(int id)
        {
            Cliente cliente = CorriereDB.GetClienteById(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        [HttpPost]
       
        public ActionResult ModificaCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                CorriereDB.UpdateCliente(cliente);
                TempData["Message"] = "Cliente modificato con successo.";
                return RedirectToAction("Index");
            }
            return View(cliente);
        }







        [HttpPost, ActionName("EliminaCliente")]
        
        public ActionResult EliminaClienteConfermato(int id)
        {
            var (success, message) = CorriereDB.EliminaCliente(id);
            TempData["Message"] = message;
            return RedirectToAction("Index");
        }


        [HttpGet]
        public JsonResult HaSpedizioniAttive(int clienteId)
        {
            bool haSpedizioni = CorriereDB.ClienteHaSpedizioniAttive(clienteId);
            return Json(haSpedizioni, JsonRequestBehavior.AllowGet);
        }








    }
}
