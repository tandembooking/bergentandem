using Microsoft.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System;

namespace TandemBooking.Tests.TestSetup
{
    public static class LocalDbTools
    {
        private static string test_db_conn_str =
            "Data Source=127.0.0.1,1433;Initial Catalog=tandembooking_test;User ID=sa;Password=Whatare5555;Encrypt=False";
        public static async Task<bool> CheckLocalDbExistsAsync(string databaseName)
        {
            //Create Database

            using (var conn = new SqlConnection(test_db_conn_str))
            {
                conn.Open();

                var checkExists = new SqlCommand("IF EXISTS( SELECT name FROM master.dbo.sysdatabases WHERE name = @name) SELECT 1 ELSE SELECT 0", conn);
                checkExists.Parameters.AddWithValue("name", databaseName);

                var result = await checkExists.ExecuteScalarAsync();
                return (int)result != 0;
            }
        }

        public static async Task<string> CreateLocalDbDatabaseAsync(string databaseName)
        {
            if (await CheckLocalDbExistsAsync(databaseName))
            {
                throw new LocalDbException(string.Format("Database {0} already exists", databaseName));
            }


            using (var conn = new SqlConnection(test_db_conn_str))
            {
                conn.Open();

                try
                {
                    var create_cmd = new SqlCommand(string.Format("CREATE DATABASE {0}", databaseName), conn);
                    await create_cmd.ExecuteNonQueryAsync();
                    //var grant_cmd = new SqlCommand(string.Format("GRANT ALL ON {0} to sa", databaseName), conn);
                    //await grant_cmd.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    throw new LocalDbException("Unable to create database, " + ex.Message, ex);
                }
            }
            return test_db_conn_str;
        }

        public static async Task DestroyLocalDbDatabase(string databaseName)
        {
            var connectionString = test_db_conn_str;
            //Get database name
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new LocalDbException(string.Format("Unable missing 'Database' in connection string '{0}'", connectionString));
            }

            //drop database
            builder["Database"] = "master";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                try
                {
                    var cmd = new SqlCommand(string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;\n DROP DATABASE [{0}]", databaseName), conn);
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    throw new LocalDbException("Unable to drop database, " + ex.Message, ex);
                }
            }
        }
    }
}