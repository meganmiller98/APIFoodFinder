using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class Ratings
    {
        public string rating { get; set; }
        public string restaurantID { get; set; }
        public string name { get; set; }
        public string userId { get; set; }

        public string error { get; set; }

        public Ratings(string rating, string restaurantID, string name, string userId, string error)
        {
            this.rating = rating;
            this.restaurantID = restaurantID;
            this.name = name;
            this.userId = userId;

            this.error = error;
        }
    }


    public class RatingsController : ApiController
    {

        [HttpGet]
        public List<Ratings> getRatings([FromUri]string ID)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT foodfinderdb.ratings.Rating, foodfinderdb.ratings.RestaurantID, foodfinderdb.users.Name, foodfinderdb.users.UserID FROM foodfinderdb.ratings  INNER JOIN users ON foodfinderdb.ratings.UserID = foodfinderdb.users.UserID  WHERE foodfinderdb.ratings.RestaurantID = @ID";
            query.Parameters.AddWithValue("@ID", ID);

            var results = new List<Ratings>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.Ratings(null, null, null, null,  ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new Ratings(fetch_query["Rating"].ToString(), fetch_query["RestaurantID"].ToString(), fetch_query["Name"].ToString(), fetch_query["UserID"].ToString(), null));
            }

            return results;
        }

        [HttpPost]
        //public void Save([FromUri]string userID, [FromUri]string restaurantID)
        public void SubmitRating([FromBody] Ratings rating)
        {
            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "INSERT INTO ratings (UserID, RestaurantID, Rating) VALUES (@userID, @restaurantID, @rating);";
            query.Parameters.AddWithValue("@userID", rating.userId);
            query.Parameters.AddWithValue("@restaurantID", rating.restaurantID);
            query.Parameters.AddWithValue("@rating", rating.rating);

            try
            {
                conn.Open();
                query.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}