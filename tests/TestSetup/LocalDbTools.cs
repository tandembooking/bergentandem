using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace TandemBooking.Tests.TestSetup
{
    public static class LocalDbTools
    {
        public static async Task<bool> CheckLocalDbExistsAsync(string databaseName)
        {
            //Create Database
            var builder = new SqlConnectionStringBuilder
            {
                ["Server"] = ".;Initial Catalog=tandembooking_test;user id=sa;password=Whatare5555",
            };
            using (var conn = new SqlConnection(builder.ConnectionString))
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

            if (File.Exists(GetLocalDbFilename(databaseName)))
            {
                throw new LocalDbException(string.Format("Database File {0} already exists", GetLocalDbFilename(databaseName)));
            }

            //Create Database
            var builder = new SqlConnectionStringBuilder();
            builder["Server"] = ".;Initial Catalog=tandembooking_test;user id=sa;password=Whatare5555";
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                try
                {
                    var cmd = new SqlCommand(string.Format("CREATE DATABASE {0} ON (name='{0}', filename='{1}')", databaseName, GetLocalDbFilename(databaseName)), conn);
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    throw new LocalDbException("Unable to create database, " + ex.Message, ex);
                }
            }

            string connectionString = GetLocalDbConnectionString(databaseName);
            return connectionString;
        }

        public static async Task DestroyLocalDbDatabase(string connectionString)
        {
            //If connectionstring does not contain an '=' sign, assume it's a database name instead
            //and build a connectionstring for it
            if (!connectionString.Contains("="))
            {
                connectionString = GetLocalDbConnectionString(connectionString);
            }

            //Get database name
            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = (string)builder["Database"];
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


        public static string GetLocalDbConnectionString(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder();
            builder["Server"] = ".;Initial Catalog=tandembooking_test;user id=sa;password=Whatare5555";
            //builder["AttachDbFilename"] = GetLocalDbFilename(databaseName);
            builder["Database"] = databaseName;
            return builder.ConnectionString;
        }

        private static string GetLocalDbFilename(string databaseName)
        {
            return string.Format(@"c:\temp\{0}.mdf", databaseName);
        }


    }
}