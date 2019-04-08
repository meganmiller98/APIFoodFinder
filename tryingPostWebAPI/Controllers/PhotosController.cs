using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class Photos
    {
        public string IDPhotos { get; set; }
        public string RestaurantID { get; set; }
        public string PhotoFilePath { get; set; }

        public string error { get; set; }

        public Photos (string IDPhotos, string RestaurantID, string PhotoFilePath, string error)
        {
            this.IDPhotos = IDPhotos;
            this.RestaurantID = RestaurantID;
            this.PhotoFilePath = PhotoFilePath;

            this.error = error;
        }
    }
    public class PhotosController : ApiController
    {
        [HttpGet]
        public List<Photos> getPhotos([FromUri]string ID)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT IDPhotos, RestaurantID, PhotoFilePath FROM photos WHERE RestaurantID = @ID";
            query.Parameters.AddWithValue("@ID", ID);

            var results = new List<Photos>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.Photos(null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new Photos(fetch_query["IDPhotos"].ToString(), fetch_query["RestaurantID"].ToString(), fetch_query["PhotoFilePath"].ToString(), null));
            }

            return results;
        }
    }
}
