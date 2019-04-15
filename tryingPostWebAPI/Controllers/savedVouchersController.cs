using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class savedVouchers
    {
        public string voucherID { get; set; }
        public string userId { get; set; }

        public string error { get; set; }

        public savedVouchers(string voucherID, string userId, string error)
        {
            this.voucherID = voucherID;
            this.userId = userId;
            this.error = error;
        }
    }
    public class savedVouchersController : ApiController
    {
        [HttpGet]
        public List<savedVouchers> checkIfSaved([FromUri]string userID, [FromUri]string voucherID)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT UserID, VoucherID FROM savedvouchers WHERE UserID = @userID AND VoucherID = @voucherID";
            query.Parameters.AddWithValue("@userID", userID);
            query.Parameters.AddWithValue("@voucherID", voucherID);

            var results = new List<savedVouchers>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.savedVouchers(null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new savedVouchers(fetch_query["UserID"].ToString(), fetch_query["VoucherID"].ToString(), null));
            }

            return results;
        }

        [HttpDelete]
        public void deleteSaved([FromUri]string userID, [FromUri]string voucherID)
        {


            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "DELETE FROM savedvouchers WHERE UserID = @userID AND VoucherID = @voucherID";
            query.Parameters.AddWithValue("@userID", userID);
            query.Parameters.AddWithValue("@voucherID", voucherID);

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

        [HttpPost]
        //public void Save([FromUri]string userID, [FromUri]string restaurantID)
        public void Save([FromBody] savedVouchers save)
        {
            //List<favedRestaurants> save = new List<favedRestaurants>();
            //save = JsonConvert.DeserializeObject<List<favedRestaurants>>(fav);

            Console.WriteLine("this is userid " + save.userId.ToString());
            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "INSERT INTO savedvouchers (VoucherID, UserID) VALUES (@voucherID, @userID);";
            query.Parameters.AddWithValue("@userID", save.userId);
            query.Parameters.AddWithValue("@voucherID", save.voucherID);

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
