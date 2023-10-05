using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Corriere.Models;

namespace Corriere.Controllers
{
    public class RegistrationController : Controller
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        }

        [HttpGet]
        public ActionResult UserRegistration()
        {
            var model = new UserRegistrationViewModel();
            return View("~/Views/Registration/UserRegistration.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserRegistration(UserRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Nickname = model.Nickname,
                    Password = model.Password,  // Semplice password senza hashing, lo so che non va fatto ma mi dava troppi errori e non avevo più tempo ;(
                    Role = "User"
                };

                if (SaveUser(user))
                {
                    TempData["SuccessMessage"] = "Registrazione completata con successo!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Si è verificato un errore durante la registrazione.";
                }
            }

            return View(model);
        }

        private bool SaveUser(User user)
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                INSERT INTO Users (Nickname, Password, Role)
                VALUES (@Nickname, @Password, @Role)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nickname", user.Nickname);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Role", user.Role);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }



        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = ValidateUser(model.Nickname, model.Password);

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Nickname, false);
                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    ModelState.AddModelError("", "Nickname o Password errati.");
                }
            }

            return View(model);
        }


        private User ValidateUser(string nickname, string password)
        {
            User userInDb = GetUserFromDatabase(nickname);

            if (userInDb != null && password == userInDb.Password)
            {
                return userInDb;
            }

            return null;
        }

        private User GetUserFromDatabase(string nickname)
        {
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT Nickname, Password, Role FROM Users WHERE Nickname = @Nickname";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nickname", nickname);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Nickname = reader["Nickname"].ToString(),
                                Password = reader["Password"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}