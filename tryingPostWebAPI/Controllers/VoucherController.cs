using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace tryingPostWebAPI.Controllers
{
    public class Vouchers
    {
        public string voucherID { get; set; }
        public string restaurantID { get; set; }
        public string expiryDate { get; set; }
        public string deal { get; set; }
        public string termsOfConditions { get; set; }
        public string voucherImage { get; set; }
        public string voucherCode { get; set; }
        public string number { get; set; }
        public string restName { get; set; }

        public string error { get; set; }
        public Vouchers(string voucherID, string restaurantID, string expiryDate, string deal, string termsOfConditions, string voucherImage, string voucherCode, string number, string restName, string error)
        {
            this.voucherID = voucherID;
            this.restaurantID = restaurantID;
            this.expiryDate = expiryDate;
            this.deal = deal;
            this.termsOfConditions = termsOfConditions;
            this.voucherImage = voucherImage;
            this.voucherCode = voucherCode;
            this.number = number;
            this.restName = restName;
            this.error = error;
        }
    }
    public class VoucherController : ApiController
    {
        [HttpGet]
        public List<Vouchers> getVouchers([FromUri]string lat, [FromUri]string lon)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT foodfinderdb.vouchers.VoucherID, foodfinderdb.vouchers.RestaurantID, foodfinderdb.vouchers.ExpiryDate, foodfinderdb.vouchers.Deal, foodfinderdb.vouchers.TermsOfConditions, foodfinderdb.vouchers.VoucherImage, foodfinderdb.vouchers.VoucherCode, foodfinderdb.restaurants.ContactTelephone, foodfinderdb.restaurants.RestaurantName, (6371 * acos(cos(radians(@lat))* cos(radians(foodfinderdb.restaurants.Latitude))* cos(radians(foodfinderdb.restaurants.Longitude) - radians(@lon))+ sin(radians(@lat))* sin(radians(foodfinderdb.restaurants.Latitude)))) AS distance FROM foodfinderdb.vouchers INNER JOIN foodfinderdb.restaurants WHERE foodfinderdb.vouchers.RestaurantID = foodfinderdb.restaurants.ID HAVING distance < 10 ORDER BY distance;";
            query.Parameters.AddWithValue("@lat", lat);
            query.Parameters.AddWithValue("@lon", lon);

            var results = new List<Vouchers>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.Vouchers(null, null, null, null, null, null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new Vouchers(fetch_query["VoucherID"].ToString(), fetch_query["RestaurantID"].ToString(), fetch_query["ExpiryDate"].ToString(), fetch_query["Deal"].ToString(), fetch_query["TermsOfConditions"].ToString(), fetch_query["VoucherImage"].ToString(), fetch_query["VoucherCode"].ToString(), fetch_query["ContactTelephone"].ToString(), fetch_query["RestaurantName"].ToString(), null));
            }

            return results;
        }

        [HttpGet]
        public List<Vouchers> getusersSavedVouchers([FromUri]string userID)
        {

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            query.CommandText = "SELECT foodfinderdb.vouchers.VoucherID, foodfinderdb.vouchers.RestaurantID, foodfinderdb.vouchers.ExpiryDate, foodfinderdb.vouchers.Deal, foodfinderdb.vouchers.TermsOfConditions, foodfinderdb.vouchers.VoucherImage, foodfinderdb.vouchers.VoucherCode, foodfinderdb.restaurants.ContactTelephone, foodfinderdb.restaurants.RestaurantName FROM foodfinderdb.vouchers INNER JOIN foodfinderdb.restaurants ON foodfinderdb.vouchers.RestaurantID = foodfinderdb.restaurants.ID INNER JOIN foodfinderdb.savedvouchers ON foodfinderdb.vouchers.VoucherID = foodfinderdb.savedvouchers.VoucherID WHERE foodfinderdb.savedvouchers.UserID = @userID;";
            query.Parameters.AddWithValue("@userID", userID);

            var results = new List<Vouchers>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.Vouchers(null, null, null, null, null, null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new Vouchers(fetch_query["VoucherID"].ToString(), fetch_query["RestaurantID"].ToString(), fetch_query["ExpiryDate"].ToString(), fetch_query["Deal"].ToString(), fetch_query["TermsOfConditions"].ToString(), fetch_query["VoucherImage"].ToString(), fetch_query["VoucherCode"].ToString(), fetch_query["ContactTelephone"].ToString(), fetch_query["RestaurantName"].ToString(), null));
            }

            return results;
        }

        [HttpDelete]
        public void deleteExpiredVouchers()
        {
            MySqlConnection con = WebApiConfig.conn();
            string sql = "SET SQL_SAFE_UPDATES = 0; delete from vouchers where ExpiryDate < NOW(); SET SQL_SAFE_UPDATES = 1;";
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
        }
    }
}
