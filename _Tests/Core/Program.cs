using System;
using System.Data.SqlClient;
using Dapper;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {        
            var db = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename =  @"d:\Work\Personal\Conferences\PASS2017\Dapper\Demo\Data\DapperSample.mdf",
                IntegratedSecurity = true,
                ConnectTimeout = 30
            };

            using(SqlConnection conn = new SqlConnection(db.ConnectionString))
            {
                conn.Query("SELECT 1 AS [Test]");
            }
            Console.WriteLine("Done!");
        }
    }
}
