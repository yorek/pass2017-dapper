using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Demos
{
    public class Helper
    {
        private static string _connectionString;

        static Helper()
        {
            var builder = new SqlConnectionStringBuilder()
            {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename = @"d:\Work\Personal\Conferences\PASS2017\Dapper\Demo\Data\DapperSample.mdf",
                IntegratedSecurity = true,
                ConnectTimeout = 30,
                ApplicationName = "DapperDemo"
            };

            _connectionString = builder.ConnectionString;
        }

        public static void RunDemo(Action<SqlConnection> demoAction)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                demoAction(conn);
            }
        }
    }
}
