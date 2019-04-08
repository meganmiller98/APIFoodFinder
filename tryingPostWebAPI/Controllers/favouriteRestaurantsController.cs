using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class favedRestaurants
    {
        public string userID { get; set; }
        public string RestaurantID { get; set; }
        
        public string error { get; set; }

        public favedRestaurants(string userID, string RestaurantID, string error)
        {
            this.RestaurantID = RestaurantID;
            this.userID = userID;
            this.error = error;
        }
    }
    public class favouriteRestaurantsController : ApiController
    {
        [HttpGet]
        public List<favedRestaurants> checkIfSaved([FromUri]string userID, [FromUri]string restaurantID)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT userID, RestaurantID FROM favouriterestaurants WHERE userID = @userID AND RestaurantID = @restaurantID";
            query.Parameters.AddWithValue("@userID", userID);
            query.Parameters.AddWithValue("@restaurantID", restaurantID);

            var results = new List<favedRestaurants>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.favedRestaurants(null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new favedRestaurants(fetch_query["userID"].ToString(), fetch_query["restaurantID"].ToString(), null));
            }

            return results;
        }
    }
}
