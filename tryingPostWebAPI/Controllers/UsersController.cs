using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class Users
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string error { get; set; }

        public Users (string UserID, string Name, string Email, string Password, string error)
        {
            this.UserID = UserID;
            this.Name = Name;
            this.Email = Email;
            this.Password = Password;

            this.error = error;
        }
    }
    public class UsersController : ApiController
    {
        [HttpGet]
        public List<Users> validateUser([FromUri]string email, string password)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT UserID, Name, Email, Password FROM users WHERE Email = @email AND Password = @password";
            query.Parameters.AddWithValue("@email", email);
            query.Parameters.AddWithValue("@password", password);

            var results = new List<Users>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.Users(null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new Users(fetch_query["UserID"].ToString(), fetch_query["Name"].ToString(), fetch_query["Email"].ToString(), fetch_query["Password"].ToString(), null));
            }

            return results;
        }

        [HttpPost]
        //public void Save([FromUri]string userID, [FromUri]string restaurantID)
        public void addUser([FromBody] Users user)
        {
            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "INSERT INTO users (Name, Email, Password) VALUES (@name, @email, @password);";
            query.Parameters.AddWithValue("@name", user.Name);
            query.Parameters.AddWithValue("@email", user.Email);
            query.Parameters.AddWithValue("@password", user.Password);

            try
            {
                conn.Open();
                query.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
    }
}
