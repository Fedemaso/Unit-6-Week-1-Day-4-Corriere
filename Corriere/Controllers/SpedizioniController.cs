using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corriere.Models;

namespace Corriere.Controllers
{
    public class SpedizioniController : Controller
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        }




        public List<SpedizioneConCliente> GetSpedizioniWithClientNames()
        {
            List<SpedizioneConCliente> spedizioniWithClientNames = new List<SpedizioneConCliente>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = @"
            SELECT S.*, C.Nome AS NomeCliente
            FROM Spedizioni S
            INNER JOIN Clienti C ON S.ClienteID = C.ClienteID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SpedizioneConCliente spedizione = new SpedizioneConCliente
                            {
                                SpedizioneID = (int)reader["SpedizioneID"],
                                NumeroSpedizione = reader["NumeroSpedizione"].ToString(),
                                DataSpedizione = (DateTime)reader["DataSpedizione"],
                                Peso = (decimal)reader["Peso"],
                                CittaDestinataria = reader["CittaDestinataria"].ToString(),
                                IndirizzoDestinatario = reader["IndirizzoDestinatario"].ToString(),
                                NominativoDestinatario = reader["NominativoDestinatario"].ToString(),
                                Costo = (decimal)reader["Costo"],
                                DataConsegnaPrevista = (DateTime)reader["DataConsegnaPrevista"],
                                NomeCliente = reader["NomeCliente"].ToString(),
                                Stato = reader["Stato"].ToString()
                            };

                            spedizioniWithClientNames.Add(spedizione);
                        }
                    }
                }
            }

            return spedizioniWithClientNames;
        }






        public ActionResult Index()
        {
            List<SpedizioneConCliente> spedizioni = GetSpedizioniWithClientNames();
            return View(spedizioni);
        }




        public ActionResult NuovaSpedizione()
        {
            ViewBag.ClienteID = new SelectList(GetClienti(), "ClienteID", "Nome");
            ViewBag.StatiPossibili = GetStatiPossibili();
            return View();

        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NuovaSpedizione([Bind(Include = "SpedizioneID,ClienteID,NumeroSpedizione,DataSpedizione,Peso,CittaDestinataria,IndirizzoDestinatario,NominativoDestinatario,Costo,DataConsegnaPrevista,Stato")] Spedizione spedizione)
        {
            if (ModelState.IsValid)
            {
                if (IsValidSqlDateTime(spedizione.DataConsegnaPrevista))
                {
                    InserisciNuovaSpedizione(spedizione);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("DataConsegnaPrevista", "La data prevista di consegna non è valida.");
                }
            }

            ViewBag.ClienteID = new SelectList(GetClienti(), "ClienteID", "Nome", spedizione.ClienteID);
            return View(spedizione);
        }



        // Funzione per verificare se una data è valida per SQL Server
        private bool IsValidSqlDateTime(DateTime value)
        {
            return (value >= SqlDateTime.MinValue.Value && value <= SqlDateTime.MaxValue.Value);
        }





        private List<string> GetStatiPossibili()
        {
            List<string> stati = new List<string>
    {
        "In transito",
        "In consegna",
        "Consegnato",
        "In Attesa"
    };
            return stati;
        }


        public ActionResult StatiPossibili()
        {
            ViewBag.StatiPossibili = GetStatiPossibili();
            return View();
        }


       

        private List<Cliente> GetClienti()
        {
            List<Cliente> clienti = new List<Cliente>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT * FROM Clienti";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cliente cliente = new Cliente
                            {
                                ClienteID = (int)reader["ClienteID"],
                                Nome = reader["Nome"].ToString(),
                            };
                            clienti.Add(cliente);
                        }
                    }
                }
            }

            return clienti;
        }





        private void InserisciNuovaSpedizione(Spedizione spedizione)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"INSERT INTO Spedizioni (ClienteID, NumeroSpedizione, DataSpedizione, Peso, CittaDestinataria, IndirizzoDestinatario, NominativoDestinatario, Costo, DataConsegnaPrevista, Stato)
                                 VALUES (@ClienteID, @NumeroSpedizione, @DataSpedizione, @Peso, @CittaDestinataria, @IndirizzoDestinatario, @NominativoDestinatario, @Costo, @DataConsegnaPrevista, @Stato)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClienteID", spedizione.ClienteID);
                    cmd.Parameters.AddWithValue("@NumeroSpedizione", spedizione.NumeroSpedizione);
                    cmd.Parameters.AddWithValue("@DataSpedizione", spedizione.DataSpedizione);
                    cmd.Parameters.AddWithValue("@Peso", spedizione.Peso);
                    cmd.Parameters.AddWithValue("@CittaDestinataria", spedizione.CittaDestinataria);
                    cmd.Parameters.AddWithValue("@IndirizzoDestinatario", spedizione.IndirizzoDestinatario);
                    cmd.Parameters.AddWithValue("@NominativoDestinatario", spedizione.NominativoDestinatario);
                    cmd.Parameters.AddWithValue("@Costo", spedizione.Costo);
                    cmd.Parameters.AddWithValue("@DataConsegnaPrevista", spedizione.DataConsegnaPrevista);
                    cmd.Parameters.AddWithValue("@Stato", spedizione.Stato);
                    cmd.ExecuteNonQuery();
                }
            }
        }





        private Spedizione GetSpedizioneById(int spedizioneID)
        {
            Spedizione spedizione = null;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Spedizioni WHERE SpedizioneID = @SpedizioneID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SpedizioneID", spedizioneID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            spedizione = new Spedizione
                            {
                                SpedizioneID = (int)reader["SpedizioneID"],
                                ClienteID = (int)reader["ClienteID"],
                                NumeroSpedizione = reader["NumeroSpedizione"].ToString(),
                                DataSpedizione = (DateTime)reader["DataSpedizione"],
                                Peso = (decimal)reader["Peso"],
                                CittaDestinataria = reader["CittaDestinataria"].ToString(),
                                IndirizzoDestinatario = reader["IndirizzoDestinatario"].ToString(),
                                NominativoDestinatario = reader["NominativoDestinatario"].ToString(),
                                Costo = (decimal)reader["Costo"],
                                DataConsegnaPrevista = (DateTime)reader["DataConsegnaPrevista"],
                                Stato = reader["Stato"].ToString()
                            };
                        }
                    }
                }
            }

            return spedizione;
        }










        private Spedizione CercaSpedizione(string codiceFiscale, string numeroSpedizione)
        {
            Spedizione spedizione = null;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Spedizioni WHERE ClienteID = @ClienteID AND NumeroSpedizione = @NumeroSpedizione";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int clienteID = GetClienteIDByCodiceFiscale(codiceFiscale);

                    if (clienteID == -1)
                    {
                        return null;
                    }

                    cmd.Parameters.AddWithValue("@ClienteID", clienteID);
                    cmd.Parameters.AddWithValue("@NumeroSpedizione", numeroSpedizione);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            spedizione = new Spedizione
                            {
                                SpedizioneID = (int)reader["SpedizioneID"],
                                ClienteID = (int)reader["ClienteID"],
                                NumeroSpedizione = reader["NumeroSpedizione"].ToString(),
                                DataSpedizione = (DateTime)reader["DataSpedizione"],
                                Peso = (decimal)reader["Peso"],
                                CittaDestinataria = reader["CittaDestinataria"].ToString(),
                                IndirizzoDestinatario = reader["IndirizzoDestinatario"].ToString(),
                                NominativoDestinatario = reader["NominativoDestinatario"].ToString(),
                                Costo = (decimal)reader["Costo"],
                                DataConsegnaPrevista = (DateTime)reader["DataConsegnaPrevista"]
                            };
                        }
                    }
                }
            }

            return spedizione;
        }






        private int GetClienteIDByCodiceFiscale(string codiceFiscale)
        {
            int clienteID = -1;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT ClienteID FROM Clienti WHERE CodiceFiscale = @CodiceFiscale";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlParameter param = new SqlParameter("@CodiceFiscale", SqlDbType.NVarChar, 4000);
                    param.Value = codiceFiscale;
                    cmd.Parameters.Add(param);

                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        clienteID = (int)result;
                    }
                }
            }

            return clienteID;
        }







        public ActionResult SpedizioniInConsegna()
        {
            return View();
        }





        public async Task<JsonResult> GetSpedizioniInConsegnaData()
        {
            DateTime today = DateTime.Today;
            List<SpedizioneInConsegna> spedizioniInConsegna = await GetSpedizioniInConsegnaFromDatabase(today);
            return Json(spedizioniInConsegna, JsonRequestBehavior.AllowGet);
        }

        private async Task<List<SpedizioneInConsegna>> GetSpedizioniInConsegnaFromDatabase(DateTime today)
        {
            List<SpedizioneInConsegna> spedizioniInConsegna = new List<SpedizioneInConsegna>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                await conn.OpenAsync();

                string query = @"
            SELECT s.NumeroSpedizione, s.DataSpedizione, s.CittaDestinataria, s.IndirizzoDestinatario, s.NominativoDestinatario, c.Nome AS NomeCliente, s.Stato
            FROM Spedizioni s
            JOIN Clienti c ON s.ClienteID = c.ClienteID
            WHERE s.Stato = 'In consegna' AND s.DataSpedizione = @Today
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Today", today);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            SpedizioneInConsegna spedizione = DataReaderSpedizioneInConsegna(reader);
                            spedizioniInConsegna.Add(spedizione);
                        }
                    }
                }
            }

            return spedizioniInConsegna;
        }

        private SpedizioneInConsegna DataReaderSpedizioneInConsegna(SqlDataReader reader)
        {
            SpedizioneInConsegna spedizione = new SpedizioneInConsegna
            {
                NumeroSpedizione = reader["NumeroSpedizione"].ToString(),
                DataSpedizione = (DateTime)reader["DataSpedizione"],
                CittaDestinataria = reader["CittaDestinataria"].ToString(),
                IndirizzoDestinatario = reader["IndirizzoDestinatario"].ToString(),
                NominativoDestinatario = reader["NominativoDestinatario"].ToString(),
                NomeCliente = reader["NomeCliente"].ToString(),
                Stato = reader["Stato"].ToString()
            };

            return spedizione;
        }




        public ActionResult SpedizioniInAttesaView()
        {
            return View("SpedizioniInAttesa");
        }



        [HttpGet]
        public async Task<JsonResult> SpedizioniInAttesa()
        {
            List<Spedizione> spedizioniInAttesa = await GetSpedizioniInAttesa();
            return Json(spedizioniInAttesa, JsonRequestBehavior.AllowGet);
        }


        private async Task<List<Spedizione>> GetSpedizioniInAttesa()
        {
            List<Spedizione> spedizioniInAttesa = new List<Spedizione>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                await conn.OpenAsync();


                string query = @"
                SELECT s.*, c.Nome AS NomeCliente
                FROM Spedizioni s
                LEFT JOIN Clienti c ON s.ClienteID = c.ClienteID
                WHERE s.Stato = 'In attesa'
            ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            Spedizione spedizione = DataReaderSpedizione(reader);
                            spedizioniInAttesa.Add(spedizione);
                        }
                    }
                }
            }

            return spedizioniInAttesa;
        }

        private Spedizione DataReaderSpedizione(SqlDataReader reader)
        {
            Spedizione spedizione = new Spedizione
            {
                SpedizioneID = (int)reader["SpedizioneID"],
                ClienteID = (int)reader["ClienteID"],
                NumeroSpedizione = reader["NumeroSpedizione"].ToString(),
                DataSpedizione = (DateTime)reader["DataSpedizione"],
                Peso = (decimal)reader["Peso"],
                CittaDestinataria = reader["CittaDestinataria"].ToString(),
                IndirizzoDestinatario = reader["IndirizzoDestinatario"].ToString(),
                NominativoDestinatario = reader["NominativoDestinatario"].ToString(),
                Costo = (decimal)reader["Costo"],
                DataConsegnaPrevista = (DateTime)reader["DataConsegnaPrevista"],
                Stato = reader["Stato"].ToString(),
                NomeCliente = reader["NomeCliente"].ToString()
            };

            return spedizione;
        }




        public ActionResult SpedizioniPerCittaView()
        {
            return View("SpedizioniPerCitta");
        }



        [HttpGet]
        public async Task<JsonResult> SpedizioniPerCitta()
        {
            List<SpedizioniPerCitta> spedizioniPerCitta = await GetSpedizioniPerCitta();
            return Json(spedizioniPerCitta, JsonRequestBehavior.AllowGet);
        }

        private async Task<List<SpedizioniPerCitta>> GetSpedizioniPerCitta()
        {
            List<SpedizioniPerCitta> spedizioniPerCitta = new List<SpedizioniPerCitta>();

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                await conn.OpenAsync();

                string query = @"
            SELECT CittaDestinataria, COUNT(*) AS NumeroSpedizioni
            FROM Spedizioni
            GROUP BY CittaDestinataria
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            SpedizioniPerCitta viewModel = new SpedizioniPerCitta
                            {
                                CittaDestinataria = reader["CittaDestinataria"].ToString(),
                                NumeroSpedizioni = (int)reader["NumeroSpedizioni"]
                            };
                            spedizioniPerCitta.Add(viewModel);
                        }
                    }
                }
            }

            return spedizioniPerCitta;
        }


        public ActionResult Dettagli(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Spedizione spedizione = GetSpedizioneById(id.Value);

            if (spedizione == null)
            {
                return HttpNotFound();
            }

            return View(spedizione);
        }




        public ActionResult ModificaSpedizione(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Spedizione spedizione = GetSpedizioneById(id.Value);
            if (spedizione == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatiPossibili = GetStatiPossibili();

            return View("ModificaSpedizione", spedizione);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificaSpedizione([Bind(Include = "SpedizioneID,ClienteID,NumeroSpedizione,DataSpedizione,Peso,CittaDestinataria,IndirizzoDestinatario,NominativoDestinatario,Costo,DataConsegnaPrevista,Stato")] Spedizione spedizione)
        {
            if (ModelState.IsValid)
            {
                UpdateSpedizione(spedizione);
                return RedirectToAction("Index");
            }
            return View("ModificaSpedizione", spedizione);
        }




        private void UpdateSpedizione(Spedizione spedizione)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
                    UPDATE Spedizioni
                    SET ClienteID = @ClienteID, 
                        NumeroSpedizione = @NumeroSpedizione, 
                        DataSpedizione = @DataSpedizione, 
                        Peso = @Peso, 
                        CittaDestinataria = @CittaDestinataria, 
                        IndirizzoDestinatario = @IndirizzoDestinatario, 
                        NominativoDestinatario = @NominativoDestinatario, 
                        Costo = @Costo, 
                        DataConsegnaPrevista = @DataConsegnaPrevista, 
                        Stato = @Stato
                    WHERE SpedizioneID = @SpedizioneID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SpedizioneID", spedizione.SpedizioneID);
                    cmd.Parameters.AddWithValue("@ClienteID", spedizione.ClienteID);
                    cmd.Parameters.AddWithValue("@NumeroSpedizione", spedizione.NumeroSpedizione);
                    cmd.Parameters.AddWithValue("@DataSpedizione", spedizione.DataSpedizione);
                    cmd.Parameters.AddWithValue("@Peso", spedizione.Peso);
                    cmd.Parameters.AddWithValue("@CittaDestinataria", spedizione.CittaDestinataria);
                    cmd.Parameters.AddWithValue("@IndirizzoDestinatario", spedizione.IndirizzoDestinatario);
                    cmd.Parameters.AddWithValue("@NominativoDestinatario", spedizione.NominativoDestinatario);
                    cmd.Parameters.AddWithValue("@Costo", spedizione.Costo);
                    cmd.Parameters.AddWithValue("@DataConsegnaPrevista", spedizione.DataConsegnaPrevista);
                    cmd.Parameters.AddWithValue("@Stato", spedizione.Stato);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Elimina(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                await conn.OpenAsync();

                string query = "DELETE FROM Spedizioni WHERE SpedizioneID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction("Index");


        }





       
            public ActionResult VerificaStato()
            {
                return View(new RicercaSpedizioneViewModel());
            }

            [HttpPost]
            public ActionResult VerificaStato(RicercaSpedizioneViewModel model)
            {
                if (ModelState.IsValid)
                {
                    Spedizione spedizione = CorriereDB.GetSpedizioneByCodiceFiscaleAndNumero(model.CodiceFiscale, model.NumeroSpedizione);
                    if (spedizione != null)
                    {
                        return View("VisualizzaStato", spedizione);
                    }
                    else
                    {
                        TempData["Error"] = "Non sono presenti spedizioni con questi dati.";
                        return RedirectToAction("VerificaStato");
                    }
                }
                return View(model);
            }
        }
    }






