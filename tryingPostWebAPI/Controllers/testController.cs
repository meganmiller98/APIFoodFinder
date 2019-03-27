using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;

namespace tryingPostWebAPI.Controllers
{
    
    public class results2
    {
        public string RestaurantName { get; set; }
        public string Address { get; set; }
        //public string times { get; set; }

        public string error { get; set; }

        //public results2(string RestaurantName, string Address, string times, string error)
        public results2(string RestaurantName, string Address, string error)
        {
            this.RestaurantName = RestaurantName;
            this.Address = Address;
            //this.times = times;
            this.error = error;
        }
    }
    public class testController : ApiController
    {
        public string time;
        public MySqlConnection con = WebApiConfig.conn();
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
        public IHttpActionResult Delete(int id)
        {
            string sql = "delete from restaurants where ID=" + id + "";
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sql, con);

                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

            }
            finally
            {
                con.Close();
            }
            return Ok();
        }

        public string day = "";
        public string day2 = "";
        [HttpGet]
        public List<results2> dayOfWeek()
        {
            int d = (int)DateTime.Now.DayOfWeek;

            if (d == 1)
            {
                day = "OpenTimesMonday";
            }

            else if (d == 2)
            {
                day = "OpenTimesTuesday";
            }
            else if (d == 3)
            {
                day = "OpenTimesWednesday";
            }
            else if (d == 4)
            {
                day = "OpenTimesThursday";
                day2 = "OpenTimesThursday";
            }
            else if (d == 5)
            {
                day = "OpenTimesFriday";
            }
            else if (d == 6)
            {
                day = "OpenTimesSaturday";
            }
            else if (d == 7)
            {
                day = "OpenTimesSunday";
            }

            Console.Write(day);

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            //query.CommandText = "SELECT RestaurantName, Address FROM restaurants";
            //query.CommandText = "SELECT Address, RestaurantName,"+ day +" FROM restaurants WHERE @day IS NOT NULL";
            query.CommandText = "SELECT Address, RestaurantName, (6371 * acos(cos(radians(-2.98268))* cos(radians(Latitude))* cos(radians(Longitude) - radians(56.456388))+ sin(radians(-2.98268))* sin(radians(Latitude)))) AS distance FROM restaurants WHERE CloseTimesThursday IS NOT NULL HAVING distance < 10 ORDER BY distance LIMIT 0 , 20;";


            //query.Parameters.AddWithValue("@day",day);
            
            var results = new List<results2>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //results.Add(new Controllers.results2(null, null, null, ex.ToString()));
                results.Add(new Controllers.results2(null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors
                //results.Add(new results2(fetch_query["RestaurantName"].ToString(), fetch_query["Address"].ToString(), fetch_query[day2].ToString(), null));
                results.Add(new results2(fetch_query["RestaurantName"].ToString(), fetch_query["Address"].ToString(), null));
            }

            return results;
        }
        
    }
}