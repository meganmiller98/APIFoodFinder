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
    
    public class results
    {
        public string RestaurantName { get; set; }
        public string Address { get; set; }

        public string error { get; set; }

        public results(string RestaurantName, string Address, string error)
        {
            this.RestaurantName = RestaurantName;
            this.Address = Address;
            this.error = error;
        }
    }
    public class ValuesController : ApiController
    {
        public MySqlConnection con = WebApiConfig.conn();

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet]
        public List<results> Get(int id)
        {
            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            //query.CommandText = "SELECT RestaurantName, Address FROM restaurants";
            query.CommandText = "SELECT RestaurantName, Address FROM restaurants WHERE ID = @id";
            query.Parameters.AddWithValue("@id", id);

            var results = new List<results>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.results(null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors
                results.Add(new results(fetch_query["RestaurantName"].ToString(), fetch_query["Address"].ToString(), null));
            }

            return results;

        }

        // POST api/values
        //public void Post([FromBody]dynamic value)
        public void Post()
        {
           

        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5

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
    }
}
