    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
using System.Web.Mvc;

    namespace Corriere.Models
    {
        public static class CorriereDB
        {
            private static string GetConnectionString()
            {
                return ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            }






            public static void AggiungiCliente(string codiceFiscale, string partitaIva, string nome, string indirizzo, string citta)
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO Clienti (CodiceFiscale, PartitaIva, Nome, Indirizzo, Citta) VALUES (@CodiceFiscale, @PartitaIva, @Nome, @Indirizzo, @Citta)";
                        cmd.Parameters.AddWithValue("@CodiceFiscale", codiceFiscale);
                        cmd.Parameters.AddWithValue("@PartitaIva", partitaIva);
                        cmd.Parameters.AddWithValue("@Nome", nome);
                        cmd.Parameters.AddWithValue("@Indirizzo", indirizzo);
                        cmd.Parameters.AddWithValue("@Citta", citta);
                        cmd.ExecuteNonQuery();
                    }
                }
            }


        public static List<Cliente> GetClienti()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT * FROM Clienti";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    List<Cliente> clienti = new List<Cliente>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cliente cliente = new Cliente
                            {
                                ClienteID = (int)reader["ClienteID"],
                                Nome = reader["Nome"].ToString(),
                                Cognome = reader["Cognome"].ToString(),
                                CodiceFiscale = reader["CodiceFiscale"].ToString(),
                                PartitaIVA = reader["PartitaIVA"].ToString()
                            };
                            clienti.Add(cliente);
                        }
                    }

                    return clienti;
                }
            }
        }


        public static Cliente GetClienteById(int clienteId)
        {
            Cliente cliente = null;

            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = "SELECT * FROM Clienti WHERE ClienteID = @ClienteID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClienteID", clienteId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new Cliente
                            {
                                ClienteID = (int)reader["ClienteID"],
                                Nome = reader["Nome"].ToString(),
                                Cognome = reader["Cognome"].ToString(),
                                CodiceFiscale = reader["CodiceFiscale"].ToString(),
                                PartitaIVA = reader["PartitaIVA"].ToString()
                            };
                        }
                    }
                }
            }

            return cliente;
        }





        public static bool UpdateCliente(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string query = @"
            UPDATE Clienti
            SET Nome = @Nome, Cognome = @Cognome, CodiceFiscale = @CodiceFiscale, PartitaIVA = @PartitaIVA
            WHERE ClienteID = @ClienteID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nome", cliente.Nome ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Cognome", cliente.Cognome ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CodiceFiscale", cliente.CodiceFiscale ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PartitaIVA", cliente.PartitaIVA ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClienteID", cliente.ClienteID);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }


        public static bool ClienteHaSpedizioniAttive(int clienteId)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Spedizioni WHERE ClienteID = @ClienteID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }


        public static (bool success, string message) EliminaCliente(int clienteId)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();

                string checkQuery = "SELECT COUNT(*) FROM Spedizioni WHERE ClienteID = @ClienteID";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@ClienteID", clienteId);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        return (false, "Il cliente ha spedizioni attive. Gestisci prima le spedizioni e poi elimina il cliente.");
                    }
                }

                string deleteQuery = "DELETE FROM Clienti WHERE ClienteID = @ClienteID";
                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@ClienteID", clienteId);
                    int rowsAffected = deleteCmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return (true, "Cliente eliminato con successo.");
                    }
                    else
                    {
                        return (false, "Errore durante l'eliminazione del cliente.");
                    }
                }
            }
        }









        public static void RegistraSpedizione(int numeroSpedizione, string codiceFiscale, string partitaIva, DateTime dataSpedizione, decimal peso, string cittaDestinataria, string indirizzoDestinatario, string nominativoDestinatario, decimal costoSpedizione, DateTime dataConsegnaPrevista)
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO Spedizioni (NumeroSpedizione, CodiceFiscale, PartitaIva, DataSpedizione, Peso, CittaDestinataria, IndirizzoDestinatario, NominativoDestinatario, CostoSpedizione, DataConsegnaPrevista) VALUES (@NumeroSpedizione, @CodiceFiscale, @PartitaIva, @DataSpedizione, @Peso, @CittaDestinataria, @IndirizzoDestinatario, @NominativoDestinatario, @CostoSpedizione, @DataConsegnaPrevista)";
                        cmd.Parameters.AddWithValue("@NumeroSpedizione", numeroSpedizione);
                        cmd.Parameters.AddWithValue("@CodiceFiscale", codiceFiscale);
                        cmd.Parameters.AddWithValue("@PartitaIva", partitaIva);
                        cmd.Parameters.AddWithValue("@DataSpedizione", dataSpedizione);
                        cmd.Parameters.AddWithValue("@Peso", peso);
                        cmd.Parameters.AddWithValue("@CittaDestinataria", cittaDestinataria);
                        cmd.Parameters.AddWithValue("@IndirizzoDestinatario", indirizzoDestinatario);
                        cmd.Parameters.AddWithValue("@NominativoDestinatario", nominativoDestinatario);
                        cmd.Parameters.AddWithValue("@CostoSpedizione", costoSpedizione);
                        cmd.Parameters.AddWithValue("@DataConsegnaPrevista", dataConsegnaPrevista);
                        cmd.ExecuteNonQuery();
                    }
                }
            }



            public static void AggiornaStatoSpedizione(int numeroSpedizione, string stato, string luogo, string descrizione, DateTime dataOraAggiornamento)
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO AggiornamentiSpedizione (NumeroSpedizione, Stato, Luogo, Descrizione, DataOraAggiornamento) VALUES (@NumeroSpedizione, @Stato, @Luogo, @Descrizione, @DataOraAggiornamento)";
                        cmd.Parameters.AddWithValue("@NumeroSpedizione", numeroSpedizione);
                        cmd.Parameters.AddWithValue("@Stato", stato);
                        cmd.Parameters.AddWithValue("@Luogo", luogo);
                        cmd.Parameters.AddWithValue("@Descrizione", descrizione);
                        cmd.Parameters.AddWithValue("@DataOraAggiornamento", dataOraAggiornamento);
                        cmd.ExecuteNonQuery();
                    }
                }
            }







        public static Spedizione GetSpedizioneByCodiceFiscaleAndNumero(string codiceFiscale, int numeroSpedizione)
        {
            Spedizione spedizione = null;
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
    SELECT * 
    FROM Spedizioni AS s
    JOIN Clienti AS c ON s.ClienteID = c.ClienteID
    WHERE c.CodiceFiscale = @CodiceFiscale AND s.NumeroSpedizione = @NumeroSpedizione
    ORDER BY s.DataSpedizione DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CodiceFiscale", codiceFiscale);
                    command.Parameters.AddWithValue("@NumeroSpedizione", numeroSpedizione);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            spedizione = new Spedizione();

                            if (!reader.IsDBNull(reader.GetOrdinal("SpedizioneID")))
                                spedizione.SpedizioneID = reader.GetInt32(reader.GetOrdinal("SpedizioneID"));

                            if (!reader.IsDBNull(reader.GetOrdinal("ClienteID")))
                                spedizione.ClienteID = reader.GetInt32(reader.GetOrdinal("ClienteID"));

                            if (!reader.IsDBNull(reader.GetOrdinal("NumeroSpedizione")))
                                spedizione.NumeroSpedizione = reader.GetString(reader.GetOrdinal("NumeroSpedizione"));

                            if (!reader.IsDBNull(reader.GetOrdinal("DataSpedizione")))
                                spedizione.DataSpedizione = reader.GetDateTime(reader.GetOrdinal("DataSpedizione"));

                            if (!reader.IsDBNull(reader.GetOrdinal("Peso")))
                                spedizione.Peso = reader.GetDecimal(reader.GetOrdinal("Peso"));

                            if (!reader.IsDBNull(reader.GetOrdinal("CittaDestinataria")))
                                spedizione.CittaDestinataria = reader.GetString(reader.GetOrdinal("CittaDestinataria"));

                            if (!reader.IsDBNull(reader.GetOrdinal("IndirizzoDestinatario")))
                                spedizione.IndirizzoDestinatario = reader.GetString(reader.GetOrdinal("IndirizzoDestinatario"));

                            if (!reader.IsDBNull(reader.GetOrdinal("NominativoDestinatario")))
                                spedizione.NominativoDestinatario = reader.GetString(reader.GetOrdinal("NominativoDestinatario"));

                            if (!reader.IsDBNull(reader.GetOrdinal("Costo")))
                                spedizione.Costo = reader.GetDecimal(reader.GetOrdinal("Costo"));

                            if (!reader.IsDBNull(reader.GetOrdinal("DataConsegnaPrevista")))
                                spedizione.DataConsegnaPrevista = reader.GetDateTime(reader.GetOrdinal("DataConsegnaPrevista"));

                            if (!reader.IsDBNull(reader.GetOrdinal("Stato")))
                                spedizione.Stato = reader.GetString(reader.GetOrdinal("Stato"));

                            if (!reader.IsDBNull(reader.GetOrdinal("Descrizione")))
                                spedizione.Descrizione = reader.GetString(reader.GetOrdinal("Descrizione"));
                        }
                    }
                }
            }

            return spedizione;
        }



    }
}
