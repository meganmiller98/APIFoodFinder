using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class MenuType
    {
        public string Menu { get; set; }
        public string RestaurantID { get; set; }

        public string error { get; set; }

        public MenuType(string Menu, string RestaurantID, string error)
        {
            this.Menu = Menu;
            this.RestaurantID = RestaurantID;

            this.error = error;
        }
    }
    public class MenuTypeController : ApiController
    {
        [HttpGet]
        public List<MenuType> getMenuTypes([FromUri]string ID)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT MenuType, RestaurantID FROM menutype WHERE RestaurantID = @ID";
            query.Parameters.AddWithValue("@ID", ID);

            var results = new List<MenuType>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.MenuType(null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new MenuType(fetch_query["MenuType"].ToString(), fetch_query["RestaurantID"].ToString(), null));
            }

            return results;
        }

    }
}
