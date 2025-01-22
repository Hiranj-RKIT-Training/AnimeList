using AnimeList.Models;
using AnimeList.Models.poco;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace AnimeList.Helper
{
    /// <summary>
    /// A helper class to manage database connections.
    /// </summary>
    public static class DatabseHelper
    {
        // Connection string fetched from the application's configuration (web.config or app.config).
        private static string _connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;

        /// <summary>
        /// Creates and opens a MySQL database connection using the connection string.
        /// </summary>
        /// <returns>A MySQL connection object.</returns>
        /// <exception cref="Exception">Throws an exception if the connection cannot be opened.</exception>
        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(_connectionString);

            try
            {
                // Attempt to open the connection
                connection.Open();
            }
            catch (Exception ex)
            {
                // If an error occurs during connection opening, throw an exception with a custom message
                throw new Exception("Error opening connection: " + ex.Message);
            }

            // Return the open connection
            return connection;
        }

        /// <summary>
        /// Creates and opens a ServiceStack IDbConnection using the connection factory.
        /// </summary>
        /// <returns>A ServiceStack IDbConnection object.</returns>
        /// <exception cref="Exception">Throws an exception if the connection cannot be opened.</exception>
        public static IDbConnection GetConnectionFactory()
        {
            // Retrieve the connection factory from the application's context
            IDbConnectionFactory connection = HttpContext.Current.Application["DbFactory"] as IDbConnectionFactory;

            try
            {
                // Open and return the connection from the factory
                return connection.OpenDbConnection();
            }
            catch (Exception ex)
            {
                // If an error occurs, throw an exception with a custom message
                throw new Exception("Error opening connection: " + ex.Message);
            }
        }
        public static Response InsertQuery<T>(T obj)
        {
            Response _objResponse = new Response();
            using (var db = GetConnectionFactory())
            {
                db.Insert<T>(obj); // Insert user into the database
                _objResponse.IsError = false;
                _objResponse.message = "User Added";

                // Generate JWT token for the new user
            }
            return _objResponse;
        }
    }
}
