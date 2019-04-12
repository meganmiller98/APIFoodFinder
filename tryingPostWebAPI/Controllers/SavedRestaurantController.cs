using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class savedRestaurants
    {
        public string userID { get; set; }
        public string RestaurantID { get; set; }
        public string RestaurantName {get; set;}
        public string MainPhoto { get; set; }

        public string error { get; set; }

        public savedRestaurants(string userID, string RestaurantID, string RestaurantName, string MainPhoto, string error)
        {
            this.RestaurantID = RestaurantID;
            this.userID = userID;
            this.RestaurantName = RestaurantName;
            this.MainPhoto = MainPhoto;
            this.error = error;
        }
    }
    

    public class SavedRestaurantController : ApiController
    {
        [HttpGet]
        public List<savedRestaurants> getSaved([FromUri]string userID)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT foodfinderdb.favouriterestaurants.userID, foodfinderdb.favouriterestaurants.RestaurantID, foodfinderdb.restaurants.RestaurantName, foodfinderdb.restaurants.MainPhoto1 FROM favouriterestaurants INNER JOIN restaurants ON foodfinderdb.favouriteRestaurants.RestaurantID = foodfinderdb.restaurants.ID WHERE userID = @userID";
            query.Parameters.AddWithValue("@userID", userID);

            var results = new List<savedRestaurants>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.savedRestaurants(null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new savedRestaurants(fetch_query["userID"].ToString(), fetch_query["restaurantID"].ToString(), fetch_query["RestaurantName"].ToString(), fetch_query["MainPhoto1"].ToString(), null));
            }

            return results;
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