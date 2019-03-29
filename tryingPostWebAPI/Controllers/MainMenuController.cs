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
    //Distance algorithm used is from gist.github.com/statickidz/8a2f0ce3bca9badbf34970b958ef8479 and stackoverflow.com/questions/1006654/fastest-way-to-find-distance-between-two-lat-long-points with some changes to the distance and display limit.

    public class results3
    {
        public string MainPhoto1 { get; set; }
        public string RestaurantName { get; set; }
        public string Categories { get; set; }
        public string Cuisines { get; set; }
        public string Address { get; set; }
        public string Opentimes { get; set; }
        public string CloseTimes { get; set; }
        public string Rating { get; set; }
        public string Cost { get; set; }

        public string error { get; set; }

        public results3(string MainPhoto1, string RestaurantName, string Categories, string Cuisines, string Address, string Opentimes, string CloseTimes, string Rating, string Cost, string error)
        {
            this.MainPhoto1 = MainPhoto1;
            this.RestaurantName = RestaurantName;
            this.Categories = Categories;
            this.Cuisines = Cuisines;
            this.Address = Address;
            this.Opentimes = Opentimes;
            this.CloseTimes = CloseTimes;
            this.Rating = Rating;
            this.Cost = Cost;
            this.error = error;
        }
    }

    public class MainMenuController : ApiController
    {
        public string day = "";
        public string closeTime = "";
        public string day2 = "";

        //localhost:52614/api/mainmenu/dayOfWeek?lon=56.456388&lat=-2.982268
        //getting the default display search screen
       
        [HttpGet]
        public List<results3> HomeResults([FromUri]string lat, [FromUri] string lon)
        {
            int d = (int)DateTime.Now.DayOfWeek;

            if (d == 1)
            {
                day = "OpenTimesMonday";
                closeTime = "CloseTimesMonday";
            }

            else if (d == 2)
            {
                day = "OpenTimesTuesday";
                closeTime = "CloseTimesTuesday";
            }
            else if (d == 3)
            {
                day = "OpenTimesWednesday";
                closeTime = "CloseTimesWednesday";
            }
            else if (d == 4)
            {
                day = "OpenTimesThursday";
                closeTime = "CloseTimesThursday";

            }
            else if (d == 5)
            {
                day = "OpenTimesFriday";
                closeTime = "CloseTimesFriday";
            }
            else if (d == 6)
            {
                day = "OpenTimesSaturday";
                closeTime = "CloseTimesSaturday";
                day2 = "OpenTimesSaturday";
            }
            else if (d == 0)
            {
                day = "OpenTimesSunday";
                closeTime = "CloseTimesSunday";
            }

            Console.Write(day);

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();

            //Distance in km
            //shows first 20 restaurants withing a 10km radius
            //the longitude and latitude are passed in as parameters from the uri.
            query.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1," + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat))* cos(radians(Latitude))* cos(radians(Longitude) - radians(@lon))+ sin(radians(@lat))* sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL HAVING distance < 10 ORDER BY distance LIMIT 0 , 20;";
            query.Parameters.AddWithValue("@lat", lat);
            query.Parameters.AddWithValue("@lon", lon);

            var results = new List<results3>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.results3(null, null, null, null, null, null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new results3(fetch_query["MainPhoto1"].ToString(), fetch_query["RestaurantName"].ToString(), fetch_query["Category"].ToString(), fetch_query["Cuisine"].ToString(), fetch_query["Address"].ToString(), fetch_query[day].ToString(), fetch_query[closeTime].ToString(), fetch_query["Rating"].ToString(), fetch_query["Cost"].ToString(), null));

            }

            return results;
        }

        //calculates the average of rating of every restaurant and temporarily stores it in averagerating table
        //inner join done with the average rating table and the restaurant table to get the average rating of each restaurant displayed in the rating column in restaurants
        //average rating table is cleared after the join
        [HttpPost]
        public void ratingsTest()
        {
            MySqlConnection con = WebApiConfig.conn();
            string sql = "INSERT INTO averageratings (RestaurantID, AverageRating) SELECT RestaurantID, AVG(Rating) FROM ratings GROUP BY RestaurantID";
            string sql2 = "SET SQL_SAFE_UPDATES = 0; UPDATE restaurants t1 INNER JOIN averageratings t2 ON t1.ID = t2.RestaurantID SET t1.rating = t2.AverageRating; SET SQL_SAFE_UPDATES = 1;";
            string sql3 = "SET SQL_SAFE_UPDATES = 0; DELETE FROM averageratings; SET SQL_SAFE_UPDATES = 0;";
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
            try
            {

                con.Open();
                MySqlCommand cmd = new MySqlCommand(sql2, con);

                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

            }
            finally
            {
                con.Close();
            }
            try
            {

                con.Open();
                MySqlCommand cmd = new MySqlCommand(sql3, con);

                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

            }
            finally
            {
                con.Close();
            }

        }

        [HttpGet]
        //public List<results3> refinements([FromUri]string sort, [FromUri]string price, [FromUri]string dietary)
        public List<results3> refinements2([FromUri]string lat, [FromUri]string lon, [FromUri]string sort, [FromUri]string dietary, [FromUri]string openNow)
        {
            string tester = DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            Console.Write("helloooooooooo" + tester);

            var refinementList = new List<results3>();

            int d = (int)DateTime.Now.DayOfWeek;

            if (d == 1)
            {
                day = "OpenTimesMonday";
                closeTime = "CloseTimesMonday";
            }

            else if (d == 2)
            {
                day = "OpenTimesTuesday";
                closeTime = "CloseTimesTuesday";
            }
            else if (d == 3)
            {
                day = "OpenTimesWednesday";
                closeTime = "CloseTimesWednesday";
            }
            else if (d == 4)
            {
                day = "OpenTimesThursday";
                closeTime = "CloseTimesThursday";

            }
            else if (d == 5)
            {
                day = "OpenTimesFriday";
                closeTime = "CloseTimesFriday";
            }
            else if (d == 6)
            {
                day = "OpenTimesSaturday";
                closeTime = "CloseTimesSaturday";
            }
            else if (d == 0)
            {
                day = "OpenTimesSunday";
                closeTime = "CloseTimesSunday";
            }
            Console.Write(day);

            MySqlConnection con = WebApiConfig.conn();
            MySqlCommand refinement = con.CreateCommand();

            if ((sort == "Distance" || sort == "distance") && dietary == "none" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL HAVING distance < 10 ORDER BY distance ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Rating" || sort == "rating") && dietary == "none" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Popular" || sort == "popular") && dietary == "none" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND Rating >= 3 HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Low" || sort == "low") && dietary == "none" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL HAVING distance < 10 ORDER BY Cost ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "High" || sort == "high") && dietary == "none" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL HAVING distance < 10 ORDER BY Cost DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Distance" || sort == "distance") && dietary == "none" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY distance ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Rating" || sort == "rating") && dietary == "none" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Popular" || sort == "popular") && dietary == "none" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND Rating >= 3 AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Low" || sort == "low") && dietary == "none" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Cost ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "High" || sort == "high") && dietary == "none" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Cost DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Distance" || sort == "distance") && dietary == "vegan" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') HAVING distance < 10 ORDER BY distance ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Rating" || sort == "rating") && dietary == "vegan" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Popular" || sort == "popular") && dietary == "vegan" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND Rating >= 3 AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Low" || sort == "low") && dietary == "vegan" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') HAVING distance < 10 ORDER BY Cost ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "High" || sort == "high") && dietary == "vegan" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') HAVING distance < 10 ORDER BY Cost DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Distance" || sort == "distance") && dietary == "vegetarian" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') HAVING distance < 10 ORDER BY distance ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Rating" || sort == "rating") && dietary == "vegetarian" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Popular" || sort == "popular") && dietary == "vegetarian" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND Rating >= 3 AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Low" || sort == "low") && dietary == "vegetarian" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') HAVING distance < 10 ORDER BY Cost ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "High" || sort == "high") && dietary == "vegetarian" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') HAVING distance < 10 ORDER BY Cost DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Distance" || sort == "distance") && dietary == "glutenfree" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') HAVING distance < 10 ORDER BY distance ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Rating" || sort == "rating") && dietary == "glutenfree" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Popular" || sort == "popular") && dietary == "glutenfree" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND Rating >= 3 AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Low" || sort == "low") && dietary == "glutenfree" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') HAVING distance < 10 ORDER BY Cost ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "High" || sort == "high") && dietary == "glutenfree" && openNow == "no")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') HAVING distance < 10 ORDER BY Cost DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            //edit open now from here
            else if ((sort == "Distance" || sort == "distance") && dietary == "vegan" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY distance ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Rating" || sort == "rating") && dietary == "vegan" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Popular" || sort == "popular") && dietary == "vegan" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND Rating >= 3 AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Low" || sort == "low") && dietary == "vegan" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Cost ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "High" || sort == "high") && dietary == "vegan" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegan') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Cost DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Distance" || sort == "distance") && dietary == "vegetarian" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY distance ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Rating" || sort == "rating") && dietary == "vegetarian" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Popular" || sort == "popular") && dietary == "vegetarian" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND Rating >= 3 AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Low" || sort == "low") && dietary == "vegetarian" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Cost ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "High" || sort == "high") && dietary == "vegetarian" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Vegetarian') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Cost DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Distance" || sort == "distance") && dietary == "glutenfree" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY distance ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Rating" || sort == "rating") && dietary == "glutenfree" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Popular" || sort == "popular") && dietary == "glutenfree" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND Rating >= 3 AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Rating DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "Low" || sort == "low") && dietary == "glutenfree" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Cost ASC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }
            else if ((sort == "High" || sort == "high") && dietary == "glutenfree" && openNow == "yes")
            {
                refinement.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat)) * cos(radians(Latitude)) * cos(radians(Longitude) - radians(@lon)) + sin(radians(@lat)) * sin(radians(Latitude)))) AS distance FROM restaurants WHERE " + day + " IS NOT NULL AND ID IN (SELECT RestaurantID FROM categories WHERE CategoryType = 'Gluten Free') AND '" + tester + "' BETWEEN " + day + " AND " + closeTime + " HAVING distance < 10 ORDER BY Cost DESC;";
                refinement.Parameters.AddWithValue("@lat", lat);
                refinement.Parameters.AddWithValue("@lon", lon);
            }

            try
            {
                con.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                refinementList.Add(new Controllers.results3(null, null, null, null, null, null, null, null, null, ex.ToString()));

            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = refinement.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                refinementList.Add(new results3(fetch_query["MainPhoto1"].ToString(), fetch_query["RestaurantName"].ToString(), fetch_query["Category"].ToString(), fetch_query["Cuisine"].ToString(), fetch_query["Address"].ToString(), fetch_query[day].ToString(), fetch_query[closeTime].ToString(), fetch_query["Rating"].ToString(), fetch_query["Cost"].ToString(), null));
                //refinementList.Add(new results3(fetch_query["MainPhoto1"].ToString(), fetch_query["RestaurantName"].ToString(), fetch_query["Category"].ToString(), fetch_query["Cuisine"].ToString(), fetch_query["Address"].ToString(), fetch_query["OpenTimesSaturday"].ToString(), fetch_query["CloseTimesSaturday"].ToString(), fetch_query["Rating"].ToString(), null));

            }
            return refinementList;
        }

        //return refinementList;
        //[FromUri]string lon, [FromUri] string lat, [FromUri] string category
        [HttpGet]
        public List<results3> GetRestaurantsAccordingToCategories([FromUri]string lon, [FromUri] string lat, [FromUri] string category)
        {
            string categoryType = category;

            int d = (int)DateTime.Now.DayOfWeek;

            if (d == 1)
            {
                day = "OpenTimesMonday";
                closeTime = "CloseTimesMonday";
            }

            else if (d == 2)
            {
                day = "OpenTimesTuesday";
                closeTime = "CloseTimesTuesday";
            }
            else if (d == 3)
            {
                day = "OpenTimesWednesday";
                closeTime = "CloseTimesWednesday";
            }
            else if (d == 4)
            {
                day = "OpenTimesThursday";
                closeTime = "CloseTimesThursday";

            }
            else if (d == 5)
            {
                day = "OpenTimesFriday";
                closeTime = "CloseTimesFriday";
            }
            else if (d == 6)
            {
                day = "OpenTimesSaturday";
                closeTime = "CloseTimesSaturday";
            }
            else if (d == 0)
            {
                day = "OpenTimesSunday";
                closeTime = "CloseTimesSunday";
            }

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();


            query.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat))* cos(radians(Latitude))* cos(radians(Longitude) - radians(@lon))+ sin(radians(@lat))* sin(radians(Latitude)))) AS distance FROM foodfinderdb.restaurants INNER JOIN foodfinderdb.categories on foodfinderdb.restaurants.ID = foodfinderdb.categories.RestaurantID WHERE foodfinderdb.categories.CategoryType = '" + categoryType+ "' AND "+day+" IS NOT NULL HAVING distance < 10 ORDER BY distance LIMIT 0 , 20";
            query.Parameters.AddWithValue("@lat", lat);
            query.Parameters.AddWithValue("@lon", lon);

            var results = new List<results3>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.results3(null, null, null, null, null, null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new results3(fetch_query["MainPhoto1"].ToString(), fetch_query["RestaurantName"].ToString(), fetch_query["Category"].ToString(), fetch_query["Cuisine"].ToString(), fetch_query["Address"].ToString(), fetch_query[day].ToString(), fetch_query[closeTime].ToString(), fetch_query["Rating"].ToString(), fetch_query["Cost"].ToString(), null));

            }

            return results;
        }

        [HttpGet]
        public List<results3> GetRestaurantsAccordingToCuisines([FromUri]string lon, [FromUri] string lat, [FromUri] string cuisine)
        {
            string cuisineType = cuisine;

            int d = (int)DateTime.Now.DayOfWeek;

            if (d == 1)
            {
                day = "OpenTimesMonday";
                closeTime = "CloseTimesMonday";
            }

            else if (d == 2)
            {
                day = "OpenTimesTuesday";
                closeTime = "CloseTimesTuesday";
            }
            else if (d == 3)
            {
                day = "OpenTimesWednesday";
                closeTime = "CloseTimesWednesday";
            }
            else if (d == 4)
            {
                day = "OpenTimesThursday";
                closeTime = "CloseTimesThursday";

            }
            else if (d == 5)
            {
                day = "OpenTimesFriday";
                closeTime = "CloseTimesFriday";
            }
            else if (d == 6)
            {
                day = "OpenTimesSaturday";
                closeTime = "CloseTimesSaturday";
            }
            else if (d == 0)
            {
                day = "OpenTimesSunday";
                closeTime = "CloseTimesSunday";
            }

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();


            query.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat))* cos(radians(Latitude))* cos(radians(Longitude) - radians(@lon))+ sin(radians(@lat))* sin(radians(Latitude)))) AS distance FROM foodfinderdb.restaurants INNER JOIN foodfinderdb.cuisines on foodfinderdb.restaurants.ID = foodfinderdb.cuisines.RestaurantID WHERE foodfinderdb.cuisines.CuisineType = '" + cuisineType + "' AND " + day + " IS NOT NULL HAVING distance < 10 ORDER BY distance LIMIT 0 , 20";
            query.Parameters.AddWithValue("@lat", lat);
            query.Parameters.AddWithValue("@lon", lon);

            var results = new List<results3>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.results3(null, null, null, null, null, null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new results3(fetch_query["MainPhoto1"].ToString(), fetch_query["RestaurantName"].ToString(), fetch_query["Category"].ToString(), fetch_query["Cuisine"].ToString(), fetch_query["Address"].ToString(), fetch_query[day].ToString(), fetch_query[closeTime].ToString(), fetch_query["Rating"].ToString(), fetch_query["Cost"].ToString(), null));

            }

            return results;
        }

        [HttpGet]
        public List<results3> GetRestaurantsAccordingToDish([FromUri]string lon, [FromUri] string lat, [FromUri] string dish)
        {
            string dishType = dish;

            int d = (int)DateTime.Now.DayOfWeek;

            if (d == 1)
            {
                day = "OpenTimesMonday";
                closeTime = "CloseTimesMonday";
            }

            else if (d == 2)
            {
                day = "OpenTimesTuesday";
                closeTime = "CloseTimesTuesday";
            }
            else if (d == 3)
            {
                day = "OpenTimesWednesday";
                closeTime = "CloseTimesWednesday";
            }
            else if (d == 4)
            {
                day = "OpenTimesThursday";
                closeTime = "CloseTimesThursday";

            }
            else if (d == 5)
            {
                day = "OpenTimesFriday";
                closeTime = "CloseTimesFriday";
            }
            else if (d == 6)
            {
                day = "OpenTimesSaturday";
                closeTime = "CloseTimesSaturday";
            }
            else if (d == 0)
            {
                day = "OpenTimesSunday";
                closeTime = "CloseTimesSunday";
            }

            MySqlConnection conn = WebApiConfig.conn();
            MySqlCommand query = conn.CreateCommand();


            query.CommandText = "SELECT Address, RestaurantName, Cuisine, Category, MainPhoto1, " + day + ", " + closeTime + ", Rating, Cost, (6371 * acos(cos(radians(@lat))* cos(radians(Latitude))* cos(radians(Longitude) - radians(@lon))+ sin(radians(@lat))* sin(radians(Latitude)))) AS distance FROM foodfinderdb.restaurants INNER JOIN foodfinderdb.menuitems on foodfinderdb.restaurants.ID = foodfinderdb.menuitems.RestaurantID WHERE foodfinderdb.menuitems.Item LIKE '%" + dishType + "%' AND " + day + " IS NOT NULL HAVING distance < 10 ORDER BY distance LIMIT 0 , 20";
            query.Parameters.AddWithValue("@lat", lat);
            query.Parameters.AddWithValue("@lon", lon);

            var results = new List<results3>();

            try
            {
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                results.Add(new Controllers.results3(null, null, null, null, null, null, null, null, null, ex.ToString()));
            }

            //provides a means of reading a forward-only stream of rows from a MySQL database
            MySqlDataReader fetch_query = query.ExecuteReader();

            while (fetch_query.Read())
            {
                //null for error parameter e.g no errors

                results.Add(new results3(fetch_query["MainPhoto1"].ToString(), fetch_query["RestaurantName"].ToString(), fetch_query["Category"].ToString(), fetch_query["Cuisine"].ToString(), fetch_query["Address"].ToString(), fetch_query[day].ToString(), fetch_query[closeTime].ToString(), fetch_query["Rating"].ToString(), fetch_query["Cost"].ToString(), null));

            }

            return results;
        }

        // GET api/<controller>
        /* public IEnumerable<string> Get()
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
         }*/

    }
}