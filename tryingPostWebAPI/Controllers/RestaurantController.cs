using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class Restaurant
    {
        public string ID { get; set; }
        public string RestaurantName { get; set; }
        public string OwnerID { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Cost { get; set; }
        public string VoucherID { get; set; }
        public string Category { get; set; }
        public string Cuisine { get; set; }
        public string MainPhoto1 { get; set; }
        public string MainPhoto2 { get; set; }
        public string MainPhoto3 { get; set; }
        public string MainPhoto4 { get; set; }
        public string MainPhoto5 { get; set; }
        public string Description { get; set; }
        public string ContactTelephone { get; set; }
        public string ContactEmail { get; set; }
        public string Website { get; set; }
        public string OpenTimesMonday { get; set; }
        public string OpenTimesTuesday { get; set; }
        public string OpenTimesWednesday { get; set; }
        public string OpenTimesThursday { get; set; }
        public string OpenTimesFriday { get; set; }
        public string OpenTimesSaturday { get; set; }
        public string OpenTimesSunday { get; set; }
        public string CloseTimesMonday { get; set; }
        public string CloseTimesTuesday { get; set; }
        public string CloseTimesWednesday { get; set; }
        public string CloseTimesThursday { get; set; }
        public string CloseTimesFriday { get; set; }
        public string CloseTimesSaturday { get; set; }
        public string CloseTimesSunday { get; set; }
        public string Rating { get; set; }


        public string error { get; set; }

        public Restaurant(string ID, string RestaurantName, string OwnerID, string Address, string Longitude, string Latitude, string Cost, string VoucherID, string Category,
            string Cuisine, string MainPhoto1, string MainPhoto2, string MainPhoto3, string MainPhoto4, string MainPhoto5, string Description,
            string ContactTelephone, string ContactEmail, string Website, string OpenTimesMonday, string OpenTimesTuesday, string OpenTimesWednesday, string OpenTimesThursday,
            string OpenTimesFriday, string OpenTimesSaturday, string OpenTimesSunday, string CloseTimesMonday, string CloseTimesTuesday,
            string CloseTimesWednesday, string CloseTimesThursday, string CloseTimesFriday, string CloseTimesSaturday, string CloseTimesSunday, string Rating, string error)
        {
            this.ID = ID;
            this.RestaurantName = RestaurantName;
            this.OwnerID = OwnerID;
            this.Address = Address;
            this.Longitude = Longitude;
            this.Latitude = Latitude;
            this.Cost = Cost;
            this.VoucherID = VoucherID;
            this.Category = Category;
            this.Cuisine = Cuisine;
            this.MainPhoto1 = MainPhoto1;
            this.MainPhoto2 = MainPhoto2;
            this.MainPhoto3 = MainPhoto3;
            this.MainPhoto4 = MainPhoto4;
            this.MainPhoto5 = MainPhoto5;
            this.Description = Description;
            this.ContactTelephone = ContactTelephone;
            this.ContactEmail = ContactEmail;
            this.Website = Website;
            this.OpenTimesMonday = OpenTimesMonday;
            this.OpenTimesTuesday = OpenTimesTuesday;
            this.OpenTimesWednesday = OpenTimesWednesday;
            this.OpenTimesThursday = OpenTimesThursday;
            this.OpenTimesFriday = OpenTimesFriday;
            this.OpenTimesSaturday = OpenTimesSaturday;
            this.OpenTimesSunday = OpenTimesSunday;
            this.CloseTimesMonday = CloseTimesMonday;
            this.CloseTimesTuesday = CloseTimesTuesday;
            this.CloseTimesWednesday = CloseTimesWednesday;
            this.CloseTimesThursday = CloseTimesThursday;
            this.CloseTimesFriday = CloseTimesFriday;
            this.CloseTimesSaturday = CloseTimesSaturday;
            this.CloseTimesSunday = CloseTimesSunday;
            this.Rating = Rating;
            this.error = error;
        }
    }

    public class RestaurantController : ApiController
    {
        [HttpGet]
        public List<Restaurant> HomeResults([FromUri]string ID)
        {
            
            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            //Distance in km
            //shows first 20 restaurants withing a 10km radius
            //the longitude and latitude are passed in as parameters from the uri.
            query.CommandText = "SELECT * FROM restaurants WHERE ID = @ID";
            query.Parameters.AddWithValue("@ID", ID);

            var results = new List<Restaurant>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.Restaurant(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new Restaurant(fetch_query["ID"].ToString(), fetch_query["RestaurantName"].ToString(), fetch_query["OwnerID"].ToString(), fetch_query["Address"].ToString(),
                    fetch_query["Longitude"].ToString(), fetch_query["Latitude"].ToString(), fetch_query["Cost"].ToString(), fetch_query["VoucherID"].ToString(),
                    fetch_query["Category"].ToString(), fetch_query["Cuisine"].ToString(), fetch_query["MainPhoto1"].ToString(), fetch_query["MainPhoto2"].ToString(),
                    fetch_query["MainPhoto3"].ToString(), fetch_query["MainPhoto4"].ToString(), fetch_query["MainPhoto5"].ToString(), fetch_query["Description"].ToString(),
                    fetch_query["ContactTelephone"].ToString(), fetch_query["ContactEmail"].ToString(), fetch_query["Website"].ToString(), fetch_query["OpenTimesMonday"].ToString(),
                    fetch_query["OpenTimesTuesday"].ToString(), fetch_query["OpenTimesWednesday"].ToString(), fetch_query["OpenTimesThursday"].ToString(), fetch_query["OpenTimesFriday"].ToString(),
                    fetch_query["OpenTimesSaturday"].ToString(), fetch_query["OpenTimesSunday"].ToString(), fetch_query["CloseTimesMonday"].ToString(), fetch_query["CloseTimesTuesday"].ToString(), fetch_query["CloseTimesWednesday"].ToString(),
                    fetch_query["CloseTimesThursday"].ToString(), fetch_query["CloseTimesFriday"].ToString(), fetch_query["CloseTimesSaturday"].ToString(), fetch_query["CloseTimesSunday"].ToString(), fetch_query["Rating"].ToString(), null));
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