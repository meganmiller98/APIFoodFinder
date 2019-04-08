using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class MenuItems
    {
        public string RestaurantID { get; set; }
        public string Item { get; set; }
        public string Price { get; set; }
        public string Description { get; set; }

        public string error { get; set; }

        public MenuItems(string RestaurantID, string Item, string Price, string Description, string error)
        {
            this.RestaurantID = RestaurantID;
            this.Item = Item;
            this.Price = Price;
            this.Description = Description;

            this.error = error;
        }
    }
    public class MenuItemsController : ApiController
    {
        [HttpGet]
        public List<MenuItems> getMenuItems([FromUri]string ID, [FromUri]string menutype)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT RestaurantID, Item, Price, Description FROM menuitems WHERE RestaurantID = @ID AND MenuType = @menutype";
            query.Parameters.AddWithValue("@ID", ID);
            query.Parameters.AddWithValue("@menutype", menutype);

            var results = new List<MenuItems>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.MenuItems(null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new MenuItems(fetch_query["RestaurantID"].ToString(), fetch_query["Item"].ToString(), fetch_query["Price"].ToString(), fetch_query["Description"].ToString(), null));
            }

            return results;
        }
    }
}
